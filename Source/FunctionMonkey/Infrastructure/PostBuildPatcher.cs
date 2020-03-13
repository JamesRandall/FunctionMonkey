using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Abstractions.Builders.Model;
using FunctionMonkey.Abstractions.Extensions;
using FunctionMonkey.Builders;
using FunctionMonkey.Commanding.Abstractions;
using FunctionMonkey.Commanding.Abstractions.Validation;
using FunctionMonkey.Commanding.Cosmos.Abstractions;
using FunctionMonkey.Extensions;
using FunctionMonkey.Model;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace FunctionMonkey.Infrastructure
{
    public class PostBuildPatcher
    {
        private readonly IMediatorResultTypeExtractor _resultTypeExtractor;

        public PostBuildPatcher(IMediatorResultTypeExtractor resultTypeExtractor)
        {
            _resultTypeExtractor = resultTypeExtractor;
        }
        
        public static void EnsureFunctionsHaveUniqueNames(IReadOnlyCollection<AbstractFunctionDefinition> functionDefinitions)
        {
            var groups = functionDefinitions.GroupBy(x => x.Name)
                .Where(x => x.Count() > 1)
                .Select(x => x.ToArray())
                .ToArray();
            foreach (var group in groups)
            {
                int index = 0;
                foreach (AbstractFunctionDefinition functionDefinition in group)
                {
                    functionDefinition.Name = $"{functionDefinition.Name}{index}";
                    index++;
                }
            }
        }

        public Type CalculateCommandResultType(AbstractFunctionDefinition definition)
        {
            if (definition.ExplicitCommandResultType != null)
            {
                return definition.ExplicitCommandResultType;
            }
                
            if (definition.NoCommandHandler || definition.CommandType.GetInterfaces().Any(x => x == typeof(ICommandWithNoHandler)))
            {
                return definition.CommandType;
            }

            return _resultTypeExtractor.CommandResultType(definition.CommandType);
        }
        
        public void Patch(FunctionHostBuilder builder, string newAssemblyNamespace)
        {
            AuthorizationBuilder authorizationBuilder = (AuthorizationBuilder) builder.AuthorizationBuilder;
            Type validationResultType = typeof(ValidationResult);
            
            foreach (AbstractFunctionDefinition definition in builder.FunctionDefinitions)
            {
                definition.Namespace = newAssemblyNamespace;
                definition.IsUsingValidator = builder.ValidatorType != null;
                definition.CommandResultType = CalculateCommandResultType(definition);
                definition.CommandDeserializerType = definition.CommandDeserializerType ??
                                                     ((SerializationBuilder)(builder.SerializationBuilder)).DefaultCommandDeserializerType;
                definition.CommandTransformerType =
                    definition.CommandTransformerType ?? builder.DefaultGlobalCommandTypeTransformer;
                if (definition.OutputBinding != null)
                {
                    definition.OutputBinding.OutputBindingConverterType =
                        definition.OutputBinding.OutputBindingConverterType ?? builder.DefaultOutputBindingConverter;
                }
                
                if (CommandRequiresNoHandler(definition.CommandType) || builder.HasNoCommandHandlers)
                {
                    definition.NoCommandHandler = true; // don't skip the if statement, this may also be set through options
                }
                if (definition.HasCommandHandlerOverride)
                {
                    definition.NoCommandHandler = false;
                }

                if (definition is HttpFunctionDefinition httpFunctionDefinition)
                {
                    CompleteHttpFunctionDefinition(builder, httpFunctionDefinition, authorizationBuilder, validationResultType);
                }
                else if (definition is CosmosDbFunctionDefinition cosmosDbFunctionDefinition)
                {
                    CompleteCosmosDbFunctionDefinition(cosmosDbFunctionDefinition);
                }
            }
        }

        private bool CommandRequiresNoHandler(Type commandType)
        {
            return commandType.GetInterfaces().Any(x => x == typeof(ICommandWithNoHandler));
        }

        private void CompleteCosmosDbFunctionDefinition(CosmosDbFunctionDefinition cosmosDbFunctionDefinition)
        {
            Type documentCommandType = typeof(ICosmosDbDocumentCommand);
            Type documentBatchCommandType = typeof(ICosmosDbDocumentBatchCommand);

            ExtractCosmosCommandProperties(cosmosDbFunctionDefinition);

            cosmosDbFunctionDefinition.IsDocumentCommand = documentCommandType.IsAssignableFrom(cosmosDbFunctionDefinition.CommandType);
            cosmosDbFunctionDefinition.IsDocumentBatchCommand = documentBatchCommandType.IsAssignableFrom(cosmosDbFunctionDefinition.CommandType);
            if (cosmosDbFunctionDefinition.IsDocumentCommand && cosmosDbFunctionDefinition.IsDocumentBatchCommand)
            {
                throw new ConfigurationException(
                    $"Command {cosmosDbFunctionDefinition.CommandType.Name} implements both ICosmosDbDocumentCommand and ICosmosDbDocumentBatchCommand - it can only implement one of these interfaces");
            }
        }

        private static void CompleteHttpFunctionDefinition(FunctionHostBuilder builder,
            HttpFunctionDefinition httpFunctionDefinition, AuthorizationBuilder authorizationBuilder,
            Type validationResultType)
        {
            if (!httpFunctionDefinition.Authorization.HasValue)
            {
                httpFunctionDefinition.Authorization = authorizationBuilder.AuthorizationDefaultValue;
            }

            if (httpFunctionDefinition.Authorization.Value == AuthorizationTypeEnum.TokenValidation)
            {
                httpFunctionDefinition.ValidatesToken = true;
            }

            if (httpFunctionDefinition.Verbs.Count == 0)
            {
                httpFunctionDefinition.Verbs.Add(HttpMethod.Get);
            }

            httpFunctionDefinition.ClaimsPrincipalAuthorizationType =
                httpFunctionDefinition.ClaimsPrincipalAuthorizationType ??
                httpFunctionDefinition.RouteConfiguration.ClaimsPrincipalAuthorizationType ??
                authorizationBuilder.DefaultClaimsPrincipalAuthorizationType;

            PatchHeaderBindings(builder, httpFunctionDefinition);
            
            httpFunctionDefinition.HttpResponseHandlerType =
                httpFunctionDefinition.HttpResponseHandlerType ?? builder.DefaultHttpResponseHandlerType;

            httpFunctionDefinition.TokenHeader = httpFunctionDefinition.TokenHeader ?? authorizationBuilder.AuthorizationHeader ?? "Authorization";

            httpFunctionDefinition.IsValidationResult = httpFunctionDefinition.CommandResultType != null &&
                                                         validationResultType.IsAssignableFrom(httpFunctionDefinition
                                                             .CommandResultType);

            httpFunctionDefinition.IsStreamCommand =
                (typeof(IStreamCommand)).IsAssignableFrom(httpFunctionDefinition.CommandType); 

            httpFunctionDefinition.TokenValidatorType = httpFunctionDefinition.TokenValidatorType ?? authorizationBuilder.TokenValidatorType;

            if (httpFunctionDefinition.ValidatesToken && httpFunctionDefinition.TokenValidatorType == null)
            {
                throw new ConfigurationException($"Command {httpFunctionDefinition.CommandType.Name} expects to be authenticated with token validation but no token validator is registered");
            }

            httpFunctionDefinition.Route = httpFunctionDefinition.Route?.TrimStart('/');

            ExtractRouteParameters(httpFunctionDefinition);

            ExtractPossibleQueryParameters(httpFunctionDefinition);

            ExtractPossibleFormParameters(httpFunctionDefinition);

            EnsureOpenApiDescription(httpFunctionDefinition);
        }

        private static void PatchHeaderBindings(FunctionHostBuilder builder, HttpFunctionDefinition httpFunctionDefinition)
        {
            if (httpFunctionDefinition.HeaderBindingConfiguration == null)
            {
                httpFunctionDefinition.HeaderBindingConfiguration = builder.DefaultHeaderBindingConfiguration;
            }
            else
            {
                if (builder.DefaultHeaderBindingConfiguration != null)
                {
                    foreach (KeyValuePair<string, string> kvp in builder.DefaultHeaderBindingConfiguration
                        .PropertyFromHeaderMappings)
                    {
                        if (!httpFunctionDefinition.HeaderBindingConfiguration.PropertyFromHeaderMappings.ContainsKey(
                            kvp.Key))
                        {
                            httpFunctionDefinition.HeaderBindingConfiguration.PropertyFromHeaderMappings
                                .Add(kvp.Key, kvp.Value);
                        }
                    }
                }
            }

            if (httpFunctionDefinition.HeaderBindingConfiguration == null)
            {
                httpFunctionDefinition.HeaderBindingConfiguration = new HeaderBindingConfiguration()
                {
                    PropertyFromHeaderMappings = new Dictionary<string, string>()
                };
            }

            if (httpFunctionDefinition.HeaderBindingConfiguration.PropertyFromHeaderMappings == null)
            {
                httpFunctionDefinition.HeaderBindingConfiguration.PropertyFromHeaderMappings = new Dictionary<string, string>();
            }
            
        }

        private static void EnsureOpenApiDescription(HttpFunctionDefinition httpFunctionDefinition)
        {
            if (httpFunctionDefinition.Route == null)
            {
                return;
            }
            // function definitions share route definitions so setting properties for one sets for all
            // but we set only if absent and based on the parent route
            // alternative would be to gather up the unique route configurations and set once but will
            // involve multiple loops
            Debug.Assert(httpFunctionDefinition.RouteConfiguration != null);
            if (string.IsNullOrWhiteSpace(httpFunctionDefinition.RouteConfiguration.OpenApiName))
            {
                string[] components = httpFunctionDefinition.RouteConfiguration.Route.Split('/');
                for (int index = components.Length - 1; index >= 0; index--)
                {
                    if (string.IsNullOrWhiteSpace(components[index]) || IsRouteParameter(components[index]))
                    {
                        continue;
                    }

                    httpFunctionDefinition.RouteConfiguration.OpenApiName = components[index];
                    break;
                }
            }
        }

        private static bool IsRouteParameter(string component)
        {
            return component.StartsWith("{") &&
                   component.EndsWith("}");
        }

        private static void ExtractCosmosCommandProperties(CosmosDbFunctionDefinition functionDefinition)
        {
            functionDefinition.CommandProperties = functionDefinition
                .CommandType
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.GetCustomAttribute<SecurityPropertyAttribute>() == null
                            && x.SetMethod != null)
                .Select(x =>
                {
                    string cosmosPropertyName = x.Name;
                    JsonPropertyAttribute jsonPropertyAttribute = x.GetCustomAttribute<JsonPropertyAttribute>();
                    if (jsonPropertyAttribute != null)
                    {
                        cosmosPropertyName = jsonPropertyAttribute.PropertyName;
                    }
                    return new CosmosDbCommandProperty
                    {
                        Name = x.Name,
                        CosmosPropertyName = cosmosPropertyName,
                        TypeName = x.PropertyType.EvaluateType(),
                        Type = x.PropertyType
                    };
                })
                .ToArray();
        }

        private static void ExtractPossibleQueryParameters(HttpFunctionDefinition httpFunctionDefinition)
        {
            Debug.Assert(httpFunctionDefinition.RouteParameters != null);

            httpFunctionDefinition.QueryParameters = httpFunctionDefinition
                .CommandType
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.GetCustomAttribute<SecurityPropertyAttribute>() == null
                            && x.SetMethod != null
                            && x.PropertyType.IsSupportedCSharpQueryParameterType()
                            && httpFunctionDefinition.RouteParameters.All(y => y.Name != x.Name) // we can't be a query parameter and a route parameter
                            )
                .Select(x => new HttpParameter
                {
                    Name = x.Name,
                    Type = x.PropertyType,
                    IsOptional = !x.PropertyType.IsValueType || Nullable.GetUnderlyingType(x.PropertyType) != null,
                    HasHeaderMapping = httpFunctionDefinition.HeaderBindingConfiguration.PropertyFromHeaderMappings.ContainsKey(x.Name)
                })
                .ToArray();
        }
        
        private static void ExtractPossibleFormParameters(HttpFunctionDefinition httpFunctionDefinition)
        {
            httpFunctionDefinition.PossibleFormProperties = httpFunctionDefinition
                .CommandType
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.GetCustomAttribute<SecurityPropertyAttribute>() == null
                            && x.SetMethod != null
                            && (x.PropertyType == typeof(IFormCollection)))
                .Select(x => new HttpParameter
                {
                    Name = x.Name,
                    Type = x.PropertyType
                })
                .ToArray();
        }

        private static void ExtractRouteParameters(HttpFunctionDefinition httpFunctionDefinition1)
        {
            List<HttpParameter> routeParameters = new List<HttpParameter>();
            if (httpFunctionDefinition1.Route == null)
            {
                httpFunctionDefinition1.RouteParameters = routeParameters;
                return;
            }
            
            PropertyInfo[] candidateCommandProperties = httpFunctionDefinition1.CommandType
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.GetCustomAttribute<SecurityPropertyAttribute>() == null
                            && x.SetMethod != null
                            && (x.PropertyType == typeof(string)
                                || Nullable.GetUnderlyingType(x.PropertyType) != null
                                || x.PropertyType.GetMethods(BindingFlags.Public | BindingFlags.Static).Any(y => y.Name == "TryParse"))).ToArray();
            Regex regex = new Regex("{(.*?)}");
            MatchCollection matches = regex.Matches(httpFunctionDefinition1.Route);
            foreach (Match match in matches)
            {
                string routeParameter = match.Groups[1].Value;
                bool isOptional = routeParameter.EndsWith("?");
                string[] routeParameterParts = routeParameter.Split(':');
                if (routeParameterParts.Length == 0)
                {
                    throw new ConfigurationException($"Bad route parameter in route {httpFunctionDefinition1.Route} for command type {httpFunctionDefinition1.CommandType.FullName}");
                }

                string routeParameterName = routeParameterParts[0].TrimEnd('?');
                PropertyInfo[] candidateProperties = candidateCommandProperties
                    .Where(p => p.Name.ToLower() == routeParameterName.ToLower()).ToArray();
                PropertyInfo matchedProperty = null;
                if (candidateProperties.Length == 1)
                {
                    matchedProperty = candidateProperties[0];
                }
                else if (candidateProperties.Length > 1)
                {
                    matchedProperty = candidateProperties.SingleOrDefault(x => x.Name == routeParameterName);
                }

                if (matchedProperty == null)
                {
                    throw new ConfigurationException($"Unable to match route parameter {routeParameterName} to property on command type {httpFunctionDefinition1.CommandType}");
                }

                bool isPropertyNullable = !matchedProperty.PropertyType.IsValueType ||
                                          Nullable.GetUnderlyingType(matchedProperty.PropertyType) != null;

                string routeTypeName;
                if (isOptional && matchedProperty.PropertyType.IsValueType &&
                    Nullable.GetUnderlyingType(matchedProperty.PropertyType) == null)
                {
                    routeTypeName = $"{matchedProperty.PropertyType.EvaluateType()}?";
                }
                else
                {
                    routeTypeName = matchedProperty.PropertyType.EvaluateType();
                }

                routeParameters.Add(new HttpParameter
                {
                    Name = matchedProperty.Name,
                    Type = matchedProperty.PropertyType,
                    IsOptional = isOptional,
                    IsNullableType = Nullable.GetUnderlyingType(matchedProperty.PropertyType) != null,
                    RouteName = routeParameterName,
                    RouteTypeName = routeTypeName
                });
            }

            httpFunctionDefinition1.RouteParameters = routeParameters;

            /*string lowerCaseRoute = httpFunctionDefinition1.Route.ToLower();
            httpFunctionDefinition1.RouteParameters = httpFunctionDefinition1
                .CommandType
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.GetCustomAttribute<SecurityPropertyAttribute>() == null
                            && x.SetMethod != null
                            && (x.PropertyType == typeof(string) || x.PropertyType
                                    .GetMethods(BindingFlags.Public | BindingFlags.Static).Any(y => y.Name == "TryParse"))
                            && lowerCaseRoute.Contains("{" + x.Name.ToLower() + "}"))
                .Select(x => new HttpParameter
                {
                    Name = x.Name,
                    TypeName = x.PropertyType.EvaluateType(),
                    Type = x.PropertyType
                })
                .ToArray();*/
        }
    }
}
