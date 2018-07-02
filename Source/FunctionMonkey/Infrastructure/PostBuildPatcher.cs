using System;
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

                    httpFunctionDefinition.TokenHeader = authorizationBuilder.AuthorizationHeader ?? "Authorization";

                    httpFunctionDefinition.IsValidationResult = httpFunctionDefinition.CommandResultType != null && validationResultType.IsAssignableFrom(httpFunctionDefinition.CommandResultType);

                    httpFunctionDefinition.PossibleQueryParameters = httpFunctionDefinition
                        .CommandType
                        .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                        .Where(x => x.GetCustomAttribute<SecurityPropertyAttribute>() == null
                                    && x.SetMethod != null
                                    && (x.PropertyType == typeof(string) || x.PropertyType.GetMethods(BindingFlags.Public | BindingFlags.Static).Any(y => y.Name == "TryParse")))
                        .Select(x => new HttpParameter
                        {
                            Name = x.Name,
                            TypeName = x.PropertyType.EvaluateType(),
                            Type = x.PropertyType
                        })
                        .ToArray();

                    string lowerCaseRoute = httpFunctionDefinition.Route.ToLower();
                    httpFunctionDefinition.RouteParameters = httpFunctionDefinition
                        .CommandType
                        .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                        .Where(x => x.GetCustomAttribute<SecurityPropertyAttribute>() == null
                                    && x.SetMethod != null
                                    && (x.PropertyType == typeof(string) || x.PropertyType.GetMethods(BindingFlags.Public | BindingFlags.Static).Any(y => y.Name == "TryParse"))
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
    }
}
