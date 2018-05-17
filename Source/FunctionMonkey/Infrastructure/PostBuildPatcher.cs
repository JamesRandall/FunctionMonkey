using System.Linq;
using System.Net.Http;
using System.Reflection;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.AzureFunctions.Abstractions.Builders;
using AzureFromTheTrenches.Commanding.AzureFunctions.Builders;
using AzureFromTheTrenches.Commanding.AzureFunctions.Extensions;
using AzureFromTheTrenches.Commanding.AzureFunctions.Model;

namespace AzureFromTheTrenches.Commanding.AzureFunctions.Infrastructure
{
    public class PostBuildPatcher
    {
        public void Patch(FunctionHostBuilder builder, string newAssemblyNamespace)
        {
            AuthorizationBuilder authorizationBuilder = (AuthorizationBuilder) builder.AuthorizationBuilder;
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

                    httpFunctionDefinition.AcceptsQueryParameters = httpFunctionDefinition
                        .CommandType
                        .GetProperties()
                        .Where(x => x.GetCustomAttribute<SecurityPropertyAttribute>() == null && x.SetMethod != null)
                        .Select(x => new HttpQueryParameter
                        {
                            Name = x.Name,
                            TypeName = x.PropertyType.EvaluateType()
                        })
                        .ToArray();
                }
            }
        }
    }
}
