using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Builders;
using FunctionMonkey.Commanding.Abstractions.Validation;
using FunctionMonkey.Extensions;
using FunctionMonkey.Model;

namespace FunctionMonkey.Infrastructure
{
    public class PostBuildPatcher
    {
        public void Patch(FunctionHostBuilder builder, string newAssemblyNamespace)
        {
            AuthorizationBuilder authorizationBuilder = (AuthorizationBuilder) builder.AuthorizationBuilder;
            Type validationResultType = typeof(ValidationResult);
            
            foreach (AbstractFunctionDefinition definition in builder.FunctionDefinitions)
            {
                definition.Namespace = newAssemblyNamespace;
                definition.IsUsingValidator = builder.ValidatorType != null;
                if (definition is HttpFunctionDefinition httpFunctionDefinition)
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

                    if (string.IsNullOrWhiteSpace(httpFunctionDefinition.ClaimsPrincipalAuthorizationTypeName))
                    {
                        httpFunctionDefinition.ClaimsPrincipalAuthorizationType = authorizationBuilder.DefaultClaimsPrincipalAuthorizationType;
                    }

                    httpFunctionDefinition.HeaderBindingConfiguration = httpFunctionDefinition.HeaderBindingConfiguration ?? builder.DefaultHeaderBindingConfiguration;

                    httpFunctionDefinition.HttpResponseHandlerType = httpFunctionDefinition.HttpResponseHandlerType ?? builder.DefaultHttpResponseHandlerType;

                    httpFunctionDefinition.TokenHeader = authorizationBuilder.AuthorizationHeader ?? "Authorization";

                    httpFunctionDefinition.IsValidationResult = httpFunctionDefinition.CommandResultType != null && validationResultType.IsAssignableFrom(httpFunctionDefinition.CommandResultType);

                    ExtractPossibleQueryParameters(httpFunctionDefinition);

                    ExtractRouteParameters(httpFunctionDefinition);

                    EnsureOpenApiDescription(httpFunctionDefinition);
                }
            }
        }

        private static void EnsureOpenApiDescription(HttpFunctionDefinition httpFunctionDefinition)
        {
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

        private static void ExtractPossibleQueryParameters(HttpFunctionDefinition httpFunctionDefinition1)
        {
            httpFunctionDefinition1.PossibleBindingProperties = httpFunctionDefinition1
                .CommandType
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.GetCustomAttribute<SecurityPropertyAttribute>() == null
                            && x.SetMethod != null
                            && (x.PropertyType == typeof(string) || x.PropertyType
                                    .GetMethods(BindingFlags.Public | BindingFlags.Static).Any(y => y.Name == "TryParse")))
                .Select(x => new HttpParameter
                {
                    Name = x.Name,
                    TypeName = x.PropertyType.EvaluateType(),
                    Type = x.PropertyType
                })
                .ToArray();
        }

        private static void ExtractRouteParameters(HttpFunctionDefinition httpFunctionDefinition1)
        {
            string lowerCaseRoute = httpFunctionDefinition1.Route.ToLower();
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
                .ToArray();
        }
    }
}
