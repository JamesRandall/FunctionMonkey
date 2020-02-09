using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Abstractions.Builders.Model;
using FunctionMonkey.Abstractions.Http;
using FunctionMonkey.Compiler.Core.Extensions;
using FunctionMonkey.Compiler.Core.Implementation.AzureFunctions;
using FunctionMonkey.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FunctionMonkey.Compiler.Core.Implementation.OpenApi
{
    internal class OpenApiCompiler
    {
        private static readonly Dictionary<HttpMethod, OperationType> MethodToOperationMap =
            new Dictionary<HttpMethod, OperationType>
            {
                {HttpMethod.Get, OperationType.Get},
                {HttpMethod.Delete, OperationType.Delete},
                {HttpMethod.Post, OperationType.Post},
                {HttpMethod.Put, OperationType.Put},
                {HttpMethod.Patch, OperationType.Patch }
            };

        public OpenApiOutputModel Compile(OpenApiConfiguration configuration, IReadOnlyCollection<AbstractFunctionDefinition> abstractFunctionDefinitions, string outputBinaryFolder)
        {
            if (configuration == null)
            {
                return null;
            }
            
            string apiPrefix = GetApiPrefix(outputBinaryFolder);

            if (!configuration.IsValid)
            {
                throw new ConfigurationException("Open API implementation is partially complete, a title and a version must be specified");
            }
            if (!configuration.IsOpenApiOutputEnabled)
            {
                return null;
            }

            HttpFunctionDefinition[] functionDefinitions = abstractFunctionDefinitions.OfType<HttpFunctionDefinition>().ToArray();
            if (functionDefinitions.Length == 0)
            {
                return null;
            }

            OpenApiDocument openApiDocument = new OpenApiDocument
            {
                Info = new OpenApiInfo
                {
                    Version = configuration.Version,
                    Title = configuration.Title
                },
                Servers = configuration.Servers?.Select(x => new OpenApiServer { Url = x }).ToArray(),
                Paths = new OpenApiPaths(),
                Components = new OpenApiComponents
                {
                    Schemas = new Dictionary<string, OpenApiSchema>()
                }
            };

            var compilerConfiguration = new OpenApiCompilerConfiguration(configuration);

            SchemaReferenceRegistry registry = new SchemaReferenceRegistry(compilerConfiguration);

            CreateTags(functionDefinitions, openApiDocument);

            CreateSchemas(functionDefinitions, openApiDocument, registry);

            CreateOperationsFromRoutes(functionDefinitions, openApiDocument, registry, apiPrefix, compilerConfiguration);

            FilterDocument(compilerConfiguration, openApiDocument);

            if (openApiDocument.Paths.Count == 0)
            {
                return null;
            }

            string yaml = openApiDocument.Serialize(OpenApiSpecVersion.OpenApi3_0, OpenApiFormat.Yaml);
            OpenApiOutputModel result = new OpenApiOutputModel
            {
                OpenApiSpecification = new OpenApiFileReference
                {
                    Content = yaml,
                    Filename = "openapi.yaml"
                }
            };

            if (!string.IsNullOrWhiteSpace(configuration.UserInterfaceRoute))
            {
                result.SwaggerUserInterface = CopySwaggerUserInterfaceFilesToWebFolder();
            }

            if (!string.IsNullOrWhiteSpace(configuration.OutputPath))
            {
                if (Directory.Exists(configuration.OutputPath))
                {
                    string pathAndFilename = Path.Combine(configuration.OutputPath, "openapi.yaml");
                    if (File.Exists(pathAndFilename))
                    {
                        File.Delete(pathAndFilename);
                    }
                    File.WriteAllText(pathAndFilename, yaml, Encoding.UTF8);
                }
            }

            return result;
        }

        private static string GetApiPrefix(string outputBinaryFolder)
        {
            string apiPrefix = "api"; // default function setting
            string hostJsonPath = Path.Combine(outputBinaryFolder, "../host.json");
            if (File.Exists(hostJsonPath))
            {
                string hostJson = File.ReadAllText(hostJsonPath);
                JObject host = JObject.Parse(hostJson);
                JObject extensions = (JObject)host["extensions"];
                if (extensions != null)
                {
                    JObject http = (JObject)extensions["http"];
                    if (http != null)
                    {
                        string hostApiPrefix = (string)http["routePrefix"];
                        if (hostApiPrefix != null)
                        {
                            apiPrefix = hostApiPrefix;
                        }
                    }
                }
            }

            return apiPrefix;
        }

        private OpenApiFileReference[] CopySwaggerUserInterfaceFilesToWebFolder()
        {
            const string prefix = "FunctionMonkey.Compiler.Core.node_modules.swagger_ui_dist.";
            Assembly sourceAssembly = GetType().Assembly;
            string[] files = sourceAssembly
                .GetManifestResourceNames()
                .Where(x => x.StartsWith(prefix))
                .ToArray();
            OpenApiFileReference[] result = new OpenApiFileReference[files.Length];
            int index = 0;
            foreach (string swaggerFile in files)
            {
                byte[] input = new byte[0];

                using (Stream inputStream = sourceAssembly.GetManifestResourceStream(swaggerFile))
                {
                    if (inputStream != null)
                    {
                        input = new byte[inputStream.Length];
                        inputStream.Read(input, 0, input.Length);
                    }
                }

                string content = Encoding.UTF8.GetString(input);

                if (swaggerFile.EndsWith(".index.html"))
                {
                    content = content.Replace("http://petstore.swagger.io/v2/swagger.json", "./openapi/openapi.yaml");
                    content = content.Replace("https://petstore.swagger.io/v2/swagger.json", "./openapi/openapi.yaml");
                    content = content.Replace("=\"./swagger", $"=\"./openapi/swagger");
                }

                result[index] = new OpenApiFileReference
                {
                    Content = content,
                    Filename = swaggerFile.Substring(prefix.Length)
                };
                index++;
            }

            return result;
        }


        private void CreateSchemas(HttpFunctionDefinition[] functionDefinitions, OpenApiDocument openApiDocument, SchemaReferenceRegistry registry)
        {
            foreach (HttpFunctionDefinition functionDefinition in functionDefinitions)
            {
                if (functionDefinition.Verbs.Contains(HttpMethod.Patch) ||
                    functionDefinition.Verbs.Contains(HttpMethod.Post) ||
                    functionDefinition.Verbs.Contains(HttpMethod.Put))
                {
                    registry.FindOrAddReference(functionDefinition.CommandType);
                }
                if (functionDefinition.CommandResultType != null &&
                    functionDefinition.CommandResultType != typeof(IActionResult))
                {
                    registry.FindOrAddReference(functionDefinition.CommandResultType);
                }
            }

            if (registry.References.Any())
            {
                openApiDocument.Components.Schemas = registry.References;
            }
        }

        private static void CreateOperationsFromRoutes(
            HttpFunctionDefinition[] functionDefinitions,
            OpenApiDocument openApiDocument,
            SchemaReferenceRegistry registry,
            string apiPrefix,
            OpenApiCompilerConfiguration compilerConfiguration)
        {
            string prependedApiPrefix = string.IsNullOrEmpty(apiPrefix) ? $"" : $"/{apiPrefix}";
            var operationsByRoute = functionDefinitions.Where(x => x.Route != null).GroupBy(x => $"{prependedApiPrefix}/{x.Route}");
            foreach (IGrouping<string, HttpFunctionDefinition> route in operationsByRoute)
            {
                OpenApiPathItem pathItem = new OpenApiPathItem()
                {
                    Operations = new Dictionary<OperationType, OpenApiOperation>()
                };

                foreach (HttpFunctionDefinition functionByRoute in route)
                {
                    Type commandType = functionByRoute.CommandType;
                    foreach (HttpMethod method in functionByRoute.Verbs)
                    {
                        OpenApiOperation operation = new OpenApiOperation
                        {
                            Description = functionByRoute.OpenApiDescription,
                            Summary = functionByRoute.OpenApiSummary,
                            Responses = new OpenApiResponses(),
                            Tags = string.IsNullOrWhiteSpace(functionByRoute.RouteConfiguration.OpenApiName) ? null : new List<OpenApiTag>() { new OpenApiTag { Name = functionByRoute.RouteConfiguration.OpenApiName } }
                        };

                        var operationFilterContext = new OpenApiOperationFilterContext
                        {
                            CommandType = commandType,
                            PropertyNames = new Dictionary<string, string>()
                        };

                        foreach (KeyValuePair<int, OpenApiResponseConfiguration> kvp in functionByRoute.OpenApiResponseConfigurations)
                        {
                            operation.Responses.Add(kvp.Key.ToString(), new OpenApiResponse
                            {
                                Description = kvp.Value.Description,
                                Content =
                                {
                                    ["application/json"] = new OpenApiMediaType()
                                    {
                                        Schema = kvp.Value.ResponseType == null ? null : registry.FindOrAddReference(kvp.Value.ResponseType)
                                    }
                                }
                            });
                        }

                        // Does any HTTP success response (2xx) exist
                        if (operation.Responses.Keys.FirstOrDefault(x => x.StartsWith("2")) == null)
                        {
                            OpenApiResponse response = new OpenApiResponse
                            {
                                Description = "Successful API operation"
                            };
                            if (functionByRoute.CommandResultType != null)
                            {
                                OpenApiSchema schema = registry.FindOrAddReference(functionByRoute.CommandResultType);
                                response.Content = new Dictionary<string, OpenApiMediaType>
                                {
                                    { "application/json", new OpenApiMediaType { Schema = schema}}
                                };
                            }
                            operation.Responses.Add("200", response);
                        }

                        if (method == HttpMethod.Get || method == HttpMethod.Delete)
                        {
                            var schema = registry.GetOrCreateSchema(commandType);
                            foreach (HttpParameter property in functionByRoute.QueryParameters)
                            {
                                var propertyInfo = commandType.GetProperty(property.Name);

                                // Property Name
                                var propertyName = propertyInfo.GetAttributeValue((JsonPropertyAttribute attribute) => attribute.PropertyName);
                                if (string.IsNullOrWhiteSpace(propertyName))
                                {
                                    propertyName = propertyInfo.GetAttributeValue((DataMemberAttribute attribute) => attribute.Name);
                                }
                                if (string.IsNullOrWhiteSpace(propertyName))
                                {
                                    propertyName = propertyInfo.Name.ToCamelCase();
                                }

                                // Property Required
                                var propertyRequired = !property.IsOptional;
                                if (!propertyRequired)
                                {
                                    propertyRequired = propertyInfo.GetAttributeValue((JsonPropertyAttribute attribute) => attribute.Required) == Required.Always;
                                }
                                if (!propertyRequired)
                                {
                                    propertyRequired = propertyInfo.GetAttributeValue((RequiredAttribute attribute) => attribute) != null;
                                }

                                var propertySchema = schema.Properties[propertyName];

                                var parameter = new OpenApiParameter
                                {
                                    Name = propertyName,
                                    In = ParameterLocation.Query,
                                    Required = propertyRequired,
                                    Schema = propertySchema, // property.Type.MapToOpenApiSchema(),
                                    Description = propertySchema.Description
                                };

                                FilterParameter(compilerConfiguration.ParameterFilters, parameter);

                                operation.Parameters.Add(parameter);
                                operationFilterContext.PropertyNames[parameter.Name] = propertyInfo.Name;
                            }
                        }

                        if (functionByRoute.Authorization == AuthorizationTypeEnum.Function && (method == HttpMethod.Get || method == HttpMethod.Delete))
                        {
                            operation.Parameters.Add(new OpenApiParameter
                            {
                                Name = "code",
                                In = ParameterLocation.Query,
                                Required = true,
                                Schema = typeof(string).MapToOpenApiSchema(),
                                Description = ""
                            });
                        }

                        foreach (HttpParameter property in functionByRoute.RouteParameters)
                        {
                            var parameter = new OpenApiParameter
                            {
                                Name = property.RouteName.ToCamelCase(),
                                In = ParameterLocation.Path,
                                Required = !property.IsOptional,
                                Schema = property.Type.MapToOpenApiSchema(),
                                Description = ""
                            };

                            FilterParameter(compilerConfiguration.ParameterFilters, parameter);

                            operation.Parameters.Add(parameter);
                            // TODO: We need to consider what to do with the payload model here - if its a route parameter
                            // we need to ignore it in the payload model                            
                        }

                        if (method == HttpMethod.Post || method == HttpMethod.Put || method == HttpMethod.Patch)
                        {
                            OpenApiRequestBody requestBody = new OpenApiRequestBody();
                            OpenApiSchema schema = registry.FindReference(commandType);
                            requestBody.Content = new Dictionary<string, OpenApiMediaType>
                            {
                                { "application/json", new OpenApiMediaType { Schema = schema }}
                            };
                            operation.RequestBody = requestBody;
                        }

                        FilterOperation(compilerConfiguration.OperationFilters, operation, operationFilterContext);

                        pathItem.Operations.Add(MethodToOperationMap[method], operation);
                    }
                }

                openApiDocument.Paths.Add(route.Key, pathItem);
            }
        }

        private static void CreateTags(HttpFunctionDefinition[] functionDefinitions, OpenApiDocument openApiDocument)
        {
            HashSet<HttpRouteConfiguration> routeConfigurations = new HashSet<HttpRouteConfiguration>();
            foreach (HttpFunctionDefinition functionDefinition in functionDefinitions)
            {
                if (functionDefinition.RouteConfiguration != null && !string.IsNullOrWhiteSpace(functionDefinition.RouteConfiguration.OpenApiName))
                {
                    routeConfigurations.Add(functionDefinition.RouteConfiguration);
                }
            }

            if (routeConfigurations.Count == 0)
            {
                return;
            }

            openApiDocument.Tags = routeConfigurations.Select(x => new OpenApiTag
            {
                Description = x.OpenApiDescription,
                Name = x.OpenApiName
            }).ToArray();
        }

        private static void FilterDocument(OpenApiCompilerConfiguration documentFilters, OpenApiDocument document)
        {
            var documentFilterContext = new OpenApiDocumentFilterContext();
            foreach (var documentFilter in documentFilters.DocumentFilters)
            {
                documentFilter.Apply(document, documentFilterContext);
            }
        }

        private static void FilterOperation(IList<IOpenApiOperationFilter> operationFilters, OpenApiOperation operation, OpenApiOperationFilterContext operationFilterContext)
        {
            foreach (var operationFilter in operationFilters)
            {
                operationFilter.Apply(operation, operationFilterContext);
            }
        }

        private static void FilterParameter(IList<IOpenApiParameterFilter> parameterFilters, OpenApiParameter parameter)
        {
            var parameterFilterContext = new OpenApiParameterFilterContext();
            foreach (var parameterFilter in parameterFilters)
            {
                parameterFilter.Apply(parameter, parameterFilterContext);
            }
        }
    }
}
