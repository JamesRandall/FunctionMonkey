using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
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
                {HttpMethod.Put, OperationType.Put}
            };

        public bool Compile(OpenApiConfiguration configuration, IReadOnlyCollection<AbstractFunctionDefinition> abstractFunctionDefinitions, string outputBinaryFolder)
        {
            if (!configuration.IsOpenApiOutputEnabled)
            {
                return false;
            }

            HttpFunctionDefinition[] functionDefinitions = abstractFunctionDefinitions.OfType<HttpFunctionDefinition>().ToArray();
            if (functionDefinitions.Length == 0)
            {
                return false;
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

            CreateTags(functionDefinitions, openApiDocument);

            CreateSchemas(functionDefinitions, openApiDocument);

            CreateOperationsFromRoutes(functionDefinitions, openApiDocument);

            if (openApiDocument.Paths.Count == 0)
            {
                return false;
            }

            string yaml = openApiDocument.Serialize(OpenApiSpecVersion.OpenApi3_0, OpenApiFormat.Yaml);
            DirectoryInfo folder = Directory.CreateDirectory(Path.Combine(outputBinaryFolder, ".."));
            string filename = Path.Combine(folder.FullName, "openapi.yaml");
            using (Stream stream = new FileStream(filename, FileMode.Create))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.Write(yaml);
            }

            if (!string.IsNullOrWhiteSpace(configuration.UserInterfaceRoute))
            {
                DirectoryInfo swaggerFolder = folder.CreateSubdirectory("swagger");
                CopySwaggerUserInterfaceFilesToWebFolder(swaggerFolder);
            }

            return true;
        }

        private void CopySwaggerUserInterfaceFilesToWebFolder(DirectoryInfo folder)
        {
            const string prefix = "FunctionMonkey.Compiler.node_modules.swagger_ui_dist.";
            Assembly sourceAssembly = GetType().Assembly;
            string[] files = sourceAssembly
                .GetManifestResourceNames()
                .Where(x => x.StartsWith(prefix))
                .ToArray();
            foreach (string swaggerFile in files)
            {
                byte[] input;
                
                using (Stream inputStream = sourceAssembly.GetManifestResourceStream(swaggerFile))
                {
                    input = new byte[inputStream.Length];
                    inputStream.Read(input, 0, input.Length);
                }

                if (swaggerFile.EndsWith(".index.html"))
                {
                    string html = Encoding.UTF8.GetString(input);
                    html = html.Replace("http://petstore.swagger.io/v2/swagger.json", "/openapi.yaml");
                    input = Encoding.UTF8.GetBytes(html);
                }

                string filename = Path.GetFileName(swaggerFile.Substring(prefix.Length));
                string outputFilename = Path.Combine(folder.FullName, filename);
                using (Stream outputStream = new FileStream(outputFilename, FileMode.Create))
                {
                    outputStream.Write(input, 0, input.Length);
                }
            }
        }


        private void CreateSchemas(HttpFunctionDefinition[] functionDefinitions, OpenApiDocument openApiDocument)
        {
            SchemaReferenceRegistry registry = new SchemaReferenceRegistry();

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
            OpenApiDocument openApiDocument)
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
                            operation.Responses.Add("200", new OpenApiResponse
                            {
                                Description = "Successful API operation"
                            });
                        }                        

                        if (functionByRoute.Route.Contains("{forumId}"))
                        {
                            operation.Parameters.Add(new OpenApiParameter
                            {
                                Name = "forumId",
                                In = ParameterLocation.Path,
                                Required = true,
                                Schema = new OpenApiSchema
                                {
                                    Type = "integer"
                                },
                                Description = ""
                            });
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
