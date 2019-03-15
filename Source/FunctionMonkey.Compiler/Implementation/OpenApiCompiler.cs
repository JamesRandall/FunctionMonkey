using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Compiler.Extensions;
using FunctionMonkey.Model;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;

namespace FunctionMonkey.Compiler.Implementation
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
                Servers = configuration.Servers?.Select(x => new OpenApiServer { Url = x}).ToArray(),
                Paths = new OpenApiPaths(),
                Components = new OpenApiComponents
                {
                    Schemas = new Dictionary<string, OpenApiSchema>()
                }
            };

            SchemaReferenceRegistry registry = new SchemaReferenceRegistry();

            CreateTags(functionDefinitions, openApiDocument);

            CreateSchemas(functionDefinitions, openApiDocument, registry);

            CreateOperationsFromRoutes(functionDefinitions, openApiDocument, registry);

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

            return result;
        }

        private OpenApiFileReference[] CopySwaggerUserInterfaceFilesToWebFolder()
        {
            const string prefix = "FunctionMonkey.Compiler.node_modules.swagger_ui_dist.";
            Assembly sourceAssembly = GetType().Assembly;
            string[] files = sourceAssembly
                .GetManifestResourceNames()
                .Where(x => x.StartsWith(prefix))
                .ToArray();
            OpenApiFileReference[] result = new OpenApiFileReference[files.Length];
            int index = 0;
            foreach (string swaggerFile in files)
            {
                byte[] input;
                
                using (Stream inputStream = sourceAssembly.GetManifestResourceStream(swaggerFile))
                {
                    input = new byte[inputStream.Length];
                    inputStream.Read(input, 0, input.Length);
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
                if (functionDefinition.Verbs.Contains(HttpMethod.Post) ||
                    functionDefinition.Verbs.Contains(HttpMethod.Put))
                {
                    registry.FindOrAddReference(functionDefinition.CommandType);
                }                
                if (functionDefinition.CommandResultType != null)
                {
                    registry.FindOrAddReference(functionDefinition.CommandResultType);
                }
            }

            if (registry.References.Any())
            {
                openApiDocument.Components.Schemas = registry.References;
            }
        }

        private static void CreateOperationsFromRoutes(HttpFunctionDefinition[] functionDefinitions,
            OpenApiDocument openApiDocument, SchemaReferenceRegistry registry)
        {
            var operationsByRoute = functionDefinitions.GroupBy(x => x.Route);
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
                            Responses = new OpenApiResponses(),
                            Tags = string.IsNullOrWhiteSpace(functionByRoute.RouteConfiguration.OpenApiName) ? null : new List<OpenApiTag>() {  new OpenApiTag {  Name = functionByRoute.RouteConfiguration.OpenApiName} }
                        };
                        foreach (KeyValuePair<int, string> kvp in functionByRoute.OpenApiResponseDescriptions)
                        {
                            operation.Responses.Add(kvp.Key.ToString(), new OpenApiResponse
                            {
                                Description = kvp.Value
                            });
                        }

                        if (!operation.Responses.ContainsKey("200"))
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

                        string lowerCaseRoute = functionByRoute.Route;
                        foreach (HttpParameter property in functionByRoute.PossibleBindingProperties)
                        {
                            if (method == HttpMethod.Get || method == HttpMethod.Delete)
                            {
                                operation.Parameters.Add(new OpenApiParameter
                                {
                                    Name = property.Name.ToCamelCase(),
                                    In = ParameterLocation.Query,
                                    Required = !property.IsOptional,
                                    Schema = property.Type.MapToOpenApiSchema(),
                                    Description = ""
                                });
                            }
                        }

                        foreach (HttpParameter property in functionByRoute.RouteParameters)
                        {
                            operation.Parameters.Add(new OpenApiParameter
                            {
                                Name = property.RouteName.ToCamelCase(),
                                In = ParameterLocation.Path,
                                Required = !property.IsOptional,
                                Schema = property.Type.MapToOpenApiSchema(),
                                Description = ""
                            });
                            // TODO: We need to consider what to do with the payload model here - if its a route parameter
                            // we need to ignore it in the payload model                            
                        }


                        if (method == HttpMethod.Post || method == HttpMethod.Put || method == HttpMethod.Patch)
                        {
                            OpenApiRequestBody requestBody = new OpenApiRequestBody();
                            OpenApiSchema schema =  registry.FindReference(commandType);
                            requestBody.Content = new Dictionary<string, OpenApiMediaType>
                            {
                                { "application/json", new OpenApiMediaType { Schema = schema}}
                            };
                            operation.RequestBody = requestBody;
                        }
                        

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
    }
}
