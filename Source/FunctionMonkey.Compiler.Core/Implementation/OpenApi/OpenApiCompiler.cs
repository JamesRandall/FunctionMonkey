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
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;

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
            string apiPrefix = GetApiPrefix(outputBinaryFolder);

            if (!configuration.IsValid)
            {
                throw new ConfigurationException("Open API implementation is partially complete, a title and a version must be specified");
            }
            if (!configuration.IsOpenApiOutputEnabled)
            {
                return null;
            }

            var functionDefinitions = abstractFunctionDefinitions.OfType<HttpFunctionDefinition>().ToList();
            if (functionDefinitions.Count() == 0)
            {
                return null;
            }

            IDictionary<string, OpenApiDocumentsSpec> openApiDocumentsSpec = new Dictionary<string, OpenApiDocumentsSpec>();
            IDictionary<string, OpenApiDocumentsSpec> redocDocumentsSpec = new Dictionary<string, OpenApiDocumentsSpec>();
            OpenApiOutputModel outputModel = new OpenApiOutputModel();
            foreach (var keyValuePair in configuration.OpenApiDocumentInfos)
            {
                OpenApiDocument openApiDocument = new OpenApiDocument
                {
                    Info = keyValuePair.Value.OpenApiInfo,
                    Servers = configuration.Servers?.Select(x => new OpenApiServer { Url = x }).ToArray(),
                    Paths = new OpenApiPaths(),
                    Components = new OpenApiComponents
                    {
                        Schemas = new Dictionary<string, OpenApiSchema>(),
                    }
                };

                var functionFilter = keyValuePair.Value.HttpFunctionFilter;
                if (functionFilter == null)
                {
                    functionFilter = new OpenApiHttpFunctionFilterDummy();
                }

                var compilerConfiguration = new OpenApiCompilerConfiguration(configuration);

                SchemaReferenceRegistry registry = new SchemaReferenceRegistry(compilerConfiguration);

                CreateTags(functionDefinitions, functionFilter, openApiDocument);

                CreateSchemas(functionDefinitions, functionFilter, openApiDocument, registry);

                CreateOperationsFromRoutes(functionDefinitions, functionFilter, openApiDocument, registry, apiPrefix, compilerConfiguration);

                CreateSecuritySchemes(openApiDocument, configuration);

                FilterDocument(compilerConfiguration.DocumentFilters, openApiDocument);

                if (openApiDocument.Paths.Count == 0)
                {
                    continue;
                }

                // TODO: FIXME:
                // Hack: Empty OpenApiSecurityRequirement lists are not serialized by the standard Microsoft
                // implementation. Therefore we add a null object to the list and fix it here by hand.
                var yaml = openApiDocument.Serialize(OpenApiSpecVersion.OpenApi3_0, OpenApiFormat.Yaml);
                yaml = Regex.Replace(yaml, $"security:\n.*?- \n", "security: []\n");

                outputModel.OpenApiFileReferences.Add(
                    new OpenApiFileReference
                    {
                        Filename = $"OpenApi.{keyValuePair.Value.DocumentRoute.Replace('/', '.')}",
                        Content = Encoding.UTF8.GetBytes(yaml)
                    }
                );

                openApiDocumentsSpec.Add(openApiDocument.Info.Title, new OpenApiDocumentsSpec
                {
                    Title = keyValuePair.Value.OpenApiInfo.Title,
                    Selected = keyValuePair.Value.Selected,
                    Path = $"/{configuration.UserInterfaceRoute??"openapi"}/{keyValuePair.Value.DocumentRoute}"
                });

                // Create redoc YAML
                if (!string.IsNullOrWhiteSpace(configuration.RedocUserInterfaceRoute))
                {
                    FilterDocument(compilerConfiguration.RedocDocumentFilters, openApiDocument);

                    // TODO: FIXME:
                    // Hack: Empty OpenApiSecurityRequirement lists are not serialized by the standard Microsoft
                    // implementation. Therefore we add a null object to the list and fix it here by hand.
                    var redocYaml = openApiDocument.Serialize(OpenApiSpecVersion.OpenApi3_0, OpenApiFormat.Yaml);
                    redocYaml = Regex.Replace(redocYaml, $"security:\n.*?- \n", "security: []\n");

                    outputModel.OpenApiFileReferences.Add(
                        new OpenApiFileReference
                        {
                            Filename = $"ReDoc.{keyValuePair.Value.DocumentRoute.Replace('/', '.')}",
                            Content = Encoding.UTF8.GetBytes(redocYaml)
                        }
                    );

                    redocDocumentsSpec.Add(openApiDocument.Info.Title, new OpenApiDocumentsSpec
                    {
                        Title = keyValuePair.Value.OpenApiInfo.Title,
                        Selected = keyValuePair.Value.Selected,
                        Path = $"/{configuration.RedocUserInterfaceRoute ?? "redoc"}/{keyValuePair.Value.DocumentRoute}"
                    });
                }
            }

            if (!string.IsNullOrWhiteSpace(configuration.UserInterfaceRoute))
            {
                outputModel.OpenApiFileReferences.Add(
                    new OpenApiFileReference
                    {
                        Filename = "OpenApi.openapi-documents-spec.json",
                        Content = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(openApiDocumentsSpec.Values.ToArray()))
                    }
                );

                CopySwaggerUserInterfaceFilesToWebFolder(configuration, outputModel.OpenApiFileReferences);
            }

            if (!string.IsNullOrWhiteSpace(configuration.RedocUserInterfaceRoute))
            {
                outputModel.OpenApiFileReferences.Add(
                    new OpenApiFileReference
                    {
                        Filename = "ReDoc.redoc-documents-spec.json",
                        Content = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(redocDocumentsSpec.Values.ToArray()))
                    }
                ) ;

                CopyRedocUserInterfaceFilesToWebFolder(configuration, outputModel.OpenApiFileReferences);
            }

            outputModel.UserInterfaceRoute = configuration.UserInterfaceRoute;
            outputModel.RedocUserInterfaceRoute = configuration.RedocUserInterfaceRoute;
            return outputModel;
        }

        private void CreateSecuritySchemes(OpenApiDocument openApiDocument, OpenApiConfiguration configuration)
        {
            foreach (KeyValuePair<string, OpenApiSecurityScheme> keyValuePair in configuration.SecuritySchemes)
            {
                // SecurityScheme
                if (openApiDocument.Components.SecuritySchemes == null)
                {
                    openApiDocument.Components.SecuritySchemes = new Dictionary<string, OpenApiSecurityScheme>();
                }
                openApiDocument.Components.SecuritySchemes.Add(keyValuePair.Key, keyValuePair.Value);

                // SecurityRequirement
                if (openApiDocument.SecurityRequirements == null)
                {
                    openApiDocument.SecurityRequirements = new List<OpenApiSecurityRequirement>();
                }
                openApiDocument.SecurityRequirements.Add(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = keyValuePair.Key,
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        new List<string>()
                    }
                });
            }
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

        private void CopySwaggerUserInterfaceFilesToWebFolder(OpenApiConfiguration configuration, IList<OpenApiFileReference> openApiFileReferences)
        {
            var route = $"{configuration.UserInterfaceRoute ?? "openapi"}";

            // StyleSheets
            StringBuilder links = new StringBuilder("");
            foreach (var injectedStylesheet in configuration.InjectedStylesheets)
            {
                var resourceAssemblyName = injectedStylesheet.resourceAssembly.GetName().Name;
                var styleSheetName = $"{resourceAssemblyName}.{injectedStylesheet.resourceName}";
                var content = LoadResourceFromAssembly(injectedStylesheet.resourceAssembly, styleSheetName);
                var filename = styleSheetName.Substring(resourceAssemblyName.Length + 1);

                links.Append($"{Environment.NewLine}    <link rel='stylesheet' type='text/css' href='/{route}/{filename}' media='{injectedStylesheet.media}' />");

                openApiFileReferences.Add(new OpenApiFileReference
                {
                    Content = content,
                    Filename = $"OpenApi.{filename}"
                });
            }
            links.Append($"{Environment.NewLine}    </head>");

            // Resources
            foreach (var injectedResource in configuration.InjectedResources)
            {
                var resourceAssemblyName = injectedResource.resourceAssembly.GetName().Name;
                var resourceName = $"{resourceAssemblyName}.{injectedResource.resourceName}";
                var content = LoadResourceFromAssembly(injectedResource.resourceAssembly, resourceName);
                var filename = resourceName.Substring(resourceAssemblyName.Length + 1);

                openApiFileReferences.Add(new OpenApiFileReference
                {
                    Content = content,
                    Filename = $"OpenApi.{filename}"
                });
            }

            // Logos
            if ((configuration.InjectedLogo) != default(ValueTuple<Assembly, string>))
            {
                var resourceAssemblyName = configuration.InjectedLogo.resourceAssembly.GetName().Name;
                var resourceName = $"{resourceAssemblyName}.{configuration.InjectedLogo.resourceName}";
                var content = LoadResourceFromAssembly(configuration.InjectedLogo.resourceAssembly, resourceName);
                var filename = resourceName.Substring(resourceAssemblyName.Length + 1);
                var extension = Path.GetExtension(filename);

                openApiFileReferences.Add(new OpenApiFileReference
                {
                    Content = content,
                    Filename = $"OpenApi.logo{extension}"
                });
            }

            // Scripts
            StringBuilder scripts = new StringBuilder();
            foreach (var injectedJavaScript in configuration.InjectedJavaScripts)
            {
                var resourceAssemblyName = injectedJavaScript.resourceAssembly.GetName().Name;
                var javaScriptName = $"{resourceAssemblyName}.{injectedJavaScript.resourceName}";
                var content = LoadResourceFromAssembly(injectedJavaScript.resourceAssembly, javaScriptName);
                var filename = javaScriptName.Substring(resourceAssemblyName.Length + 1);

                scripts.Append($"    <script src=\"/{route}/{filename}\" > </script>{ Environment.NewLine}");

                openApiFileReferences.Add(new OpenApiFileReference
                {
                    Content = content,
                    Filename = $"OpenApi.{filename}"
                });
            }

            // Additional necessary scripts
            var necessaryScripts = new List<string>();
            necessaryScripts.Add("Resources.OpenApi.topbar-multiple-specs.js");
            foreach (var resourceName in necessaryScripts)
            {
                var resourceAssemblyName = Assembly.GetExecutingAssembly().GetName().Name;
                var javaScriptName = $"{resourceAssemblyName}.{resourceName}";
                var content = LoadResourceFromAssembly(Assembly.GetExecutingAssembly(), javaScriptName);
                var filename = javaScriptName.Substring(resourceAssemblyName.Length + 1);

                scripts.Append($"    <script src=\"/{route}/{filename.Replace("Resources.OpenApi.", "")}\" > </script>{ Environment.NewLine}");

                openApiFileReferences.Add(new OpenApiFileReference
                {
                    Content = content,
                    Filename = filename.Replace("Resources.OpenApi.", "OpenApi.")
                });
            }
            scripts.Append("  </body>");

            // Other necessary files
            var prefix = "FunctionMonkey.Compiler.Core.node_modules.swagger_ui_dist.";
            Assembly sourceAssembly = GetType().Assembly;
            var necessaryFiles = sourceAssembly
                .GetManifestResourceNames()
                .Where(x => x.StartsWith(prefix)).Select(name => (prefix, name))
                .ToList();
            prefix = "FunctionMonkey.Compiler.Core.Resources.OpenApi.";
            necessaryFiles.Add((prefix, "FunctionMonkey.Compiler.Core.Resources.OpenApi.swagger-logo.svg"));
            foreach (var necessaryFile in necessaryFiles)
            {
                var content = LoadResourceFromAssembly(sourceAssembly, necessaryFile.name);

                if (necessaryFile.name.EndsWith(".index.html"))
                {
                    var documentInfo = configuration.OpenApiDocumentInfos
                        .Where(x => x.Value.Selected)
                        .Select(x => x.Value)
                        .DefaultIfEmpty(configuration.OpenApiDocumentInfos.FirstOrDefault().Value)
                        .First();

                    var contentString = Encoding.UTF8.GetString(content);
                    contentString = contentString.Replace("http://petstore.swagger.io/v2/swagger.json", $"/{route}/{documentInfo.DocumentRoute}");
                    contentString = contentString.Replace("https://petstore.swagger.io/v2/swagger.json", $"/{route}/{documentInfo.DocumentRoute}");
                    contentString = contentString.Replace("=\"./swagger", $"=\"/{configuration.UserInterfaceRoute}/swagger");
                    contentString = contentString.Replace("href=\"./favicon-", "href=\"/openapi/favicon-");
                    contentString = contentString.Replace("</head>", links.ToString());
                    contentString = contentString.Replace("</body>", scripts.ToString());
                    content = Encoding.UTF8.GetBytes(contentString);
                }

                openApiFileReferences.Add(new OpenApiFileReference
                {
                    Content = content,
                    Filename = $"OpenApi.{necessaryFile.name.Substring(necessaryFile.prefix.Length)}"
                });
            }
        }

        private void CopyRedocUserInterfaceFilesToWebFolder(OpenApiConfiguration configuration, IList<OpenApiFileReference> openApiFileReferences)
        {
            var route = $"{configuration.RedocUserInterfaceRoute ?? "redoc"}";

            // StyleSheets
            StringBuilder links = new StringBuilder("");
            foreach (var injectedStylesheet in configuration.RedocInjectedStylesheets)
            {
                var resourceAssemblyName = injectedStylesheet.resourceAssembly.GetName().Name;
                var styleSheetName = $"{resourceAssemblyName}.{injectedStylesheet.resourceName}";
                var content = LoadResourceFromAssembly(injectedStylesheet.resourceAssembly, styleSheetName);
                var filename = styleSheetName.Substring(resourceAssemblyName.Length + 1);

                links.Append($"{Environment.NewLine}    <link rel='stylesheet' type='text/css' href='/{route}/{filename}' media='{injectedStylesheet.media}' />");

                openApiFileReferences.Add(new OpenApiFileReference
                {
                    Content = content,
                    Filename = $"/ReDoc.{filename}"
                });
            }
            links.Append($"{Environment.NewLine}</head>");

            // Resources
            foreach (var injectedResource in configuration.RedocInjectedResources)
            {
                var resourceAssemblyName = injectedResource.resourceAssembly.GetName().Name;
                var resourceName = $"{resourceAssemblyName}.{injectedResource.resourceName}";
                var content = LoadResourceFromAssembly(injectedResource.resourceAssembly, resourceName);
                var filename = resourceName.Substring(resourceAssemblyName.Length + 1);

                openApiFileReferences.Add(new OpenApiFileReference
                {
                    Content = content,
                    Filename = $"/ReDoc.{filename}"
                });
            }

            // Logo
            if (configuration.RedocInjectedLogo != default(ValueTuple<Assembly, string>))
            {
                var resourceAssemblyName = configuration.InjectedLogo.resourceAssembly.GetName().Name;
                var resourceName = $"{resourceAssemblyName}.{configuration.InjectedLogo.resourceName}";
                var content = LoadResourceFromAssembly(configuration.InjectedLogo.resourceAssembly, resourceName);
                var filename = resourceName.Substring(resourceAssemblyName.Length + 1);
                var extension = Path.GetExtension(filename);

                openApiFileReferences.Add(new OpenApiFileReference
                {
                    Content = content,
                    Filename = $"Redoc.logo{extension}"
                });
            }

            // Scripts
            StringBuilder scripts = new StringBuilder();
            foreach (var injectedJavaScript in configuration.RedocInjectedJavaScripts)
            {
                var resourceAssemblyName = injectedJavaScript.resourceAssembly.GetName().Name;
                var javaScriptName = $"{resourceAssemblyName}.{injectedJavaScript.resourceName}";
                var content = LoadResourceFromAssembly(injectedJavaScript.resourceAssembly, javaScriptName);
                var filename = javaScriptName.Substring(resourceAssemblyName.Length + 1);

                scripts.Append($"    <script src=\"/{route}/{filename}\" > </script>{ Environment.NewLine}");

                openApiFileReferences.Add(new OpenApiFileReference
                {
                    Content = content,
                    Filename = $"/ReDoc.{filename}"
                });
            }

            // Additional necessary scripts
            var necessaryScripts = new List<string>();
            necessaryScripts.Add("Resources.Redoc.topbar-multiple-specs.js");
            foreach (var resourceName in necessaryScripts)
            {
                var resourceAssemblyName = Assembly.GetExecutingAssembly().GetName().Name;
                var javaScriptName = $"{resourceAssemblyName}.{resourceName}";
                var content = LoadResourceFromAssembly(Assembly.GetExecutingAssembly(), javaScriptName);
                var filename = javaScriptName.Substring(resourceAssemblyName.Length + 1);

                scripts.Append($"    <script src=\"/{route}/{filename.Replace("Resources.Redoc.", "")}\" > </script>{ Environment.NewLine}");

                openApiFileReferences.Add(new OpenApiFileReference
                {
                    Content = content,
                    Filename = filename.Replace("Resources.Redoc.", "ReDoc.")
                });
            }
            scripts.Append("</body>");

            // Other necessary files
            const string prefix = "Resources.Redoc.";
            var necessaryFiles = new List<string>();
            necessaryFiles.Add("Resources.Redoc.index.html");
            necessaryFiles.Add("Resources.Redoc.redoc-logo.png");
            foreach (var resourceName in necessaryFiles)
            {
                var resourceAssemblyName = Assembly.GetExecutingAssembly().GetName().Name;
                var redocFileName = $"{resourceAssemblyName}.{resourceName}";
                var content = LoadResourceFromAssembly(Assembly.GetExecutingAssembly(), redocFileName);
                var filename = redocFileName.Substring(resourceAssemblyName.Length + 1);

                if (filename.EndsWith("index.html"))
                {
                    var documentInfo = configuration.OpenApiDocumentInfos
                        .Where(x => x.Value.Selected)
                        .Select(x => x.Value)
                        .DefaultIfEmpty(configuration.OpenApiDocumentInfos.FirstOrDefault().Value)
                        .First();

                    var contentString = Encoding.UTF8.GetString(content);
                    contentString = contentString.Replace("/redoc/openapi.yaml", $"/{route}/{documentInfo.DocumentRoute}");
                    contentString = contentString.Replace("</head>", links.ToString());
                    contentString = contentString.Replace("</body>", scripts.ToString());

                        //< link rel = "icon" type = "image/png" href = "/openapi/favicon-32x32.png" sizes = "32x32" />
       
                        //< link rel = "icon" type = "image/png" href = "/openapi/favicon-16x16.png" sizes = "16x16" />

                                                                                                           content = Encoding.UTF8.GetBytes(contentString);
                }

                openApiFileReferences.Add(new OpenApiFileReference
                {
                    Content = content,
                    Filename = $"ReDoc.{filename.Substring(prefix.Length)}"
                });
            }
        }

        private byte[] LoadResourceFromAssembly(Assembly assembly, string resourceName)
        {
            byte[] input = new byte[0];

            using (Stream inputStream = assembly.GetManifestResourceStream(resourceName))
            {
                if (inputStream != null)
                {
                    input = new byte[inputStream.Length];
                    inputStream.Read(input, 0, input.Length);
                }
            }

            return input;
        }

        private void CreateSchemas(IList<HttpFunctionDefinition> functionDefinitions, IOpenApiHttpFunctionFilter functionFilter, OpenApiDocument openApiDocument, SchemaReferenceRegistry registry)
        {
            IOpenApiHttpFunctionFilterContext functionFilterContext = new OpenApiHttpFunctionFilterContext();
            foreach (HttpFunctionDefinition functionDefinition in functionDefinitions)
            {
                var filterdVerbs = new HashSet<HttpMethod>(functionDefinition.Verbs);
                functionFilter.Apply(functionDefinition.RouteConfiguration.Route, filterdVerbs, functionFilterContext);
                if (filterdVerbs.Count == 0)
                {
                    continue;
                }

                if (filterdVerbs.Contains(HttpMethod.Patch) ||
                    filterdVerbs.Contains(HttpMethod.Post) ||
                    filterdVerbs.Contains(HttpMethod.Put))
                {
                    registry.FindOrAddReference(functionDefinition.CommandType);
                }
                if (functionDefinition.CommandResultType != null &&
                    functionDefinition.CommandResultType != typeof(IActionResult))
                {
                    registry.FindOrAddReference(functionDefinition.CommandResultType);
                }
            }

            openApiDocument.Components.Schemas = registry.References;
        }

        private static void CreateOperationsFromRoutes(
            IList<HttpFunctionDefinition> functionDefinitions,
            IOpenApiHttpFunctionFilter functionFilter,
            OpenApiDocument openApiDocument,
            SchemaReferenceRegistry registry,
            string apiPrefix,
            OpenApiCompilerConfiguration compilerConfiguration)
        {
            IOpenApiHttpFunctionFilterContext functionFilterContext = new OpenApiHttpFunctionFilterContext();
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
                    if (functionByRoute.OpenApiIgnore)
                    {
                        continue;
                    }

                    var filterdVerbs = new HashSet<HttpMethod>(functionByRoute.Verbs);
                    functionFilter.Apply(functionByRoute.RouteConfiguration.Route, filterdVerbs, functionFilterContext);
                    if (filterdVerbs.Count == 0)
                    {
                        continue;
                    }

                    Type commandType = functionByRoute.CommandType;
                    foreach (HttpMethod method in filterdVerbs)
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

                        // TODO: FIXME:
                        // Empty OpenApiSecurityRequirement lists are not serialized by the standard Microsoft
                        // implementation. Therefore we add a null object to the list here and fix it later by hand.
                        if (functionByRoute.Authorization == AuthorizationTypeEnum.Anonymous)
                        {
                            operation.Security.Add(null);
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

        private static void CreateTags(IList<HttpFunctionDefinition> functionDefinitions, IOpenApiHttpFunctionFilter functionFilter, OpenApiDocument openApiDocument)
        {
            IOpenApiHttpFunctionFilterContext functionFilterContext = new OpenApiHttpFunctionFilterContext();
            HashSet<HttpRouteConfiguration> routeConfigurations = new HashSet<HttpRouteConfiguration>();
            foreach (HttpFunctionDefinition functionDefinition in functionDefinitions)
            {
                if (!string.IsNullOrWhiteSpace(functionDefinition.RouteConfiguration?.OpenApiName))
                {
                    var filterdVerbs = new HashSet<HttpMethod>(functionDefinition.Verbs);
                    functionFilter.Apply(functionDefinition.RouteConfiguration.Route, filterdVerbs, functionFilterContext);
                    if (filterdVerbs.Count != 0)
                    {
                        routeConfigurations.Add(functionDefinition.RouteConfiguration);
                    }
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
            }).ToList();
        }

        private void FilterDocument(IList<IOpenApiDocumentFilter> documentFilters, OpenApiDocument document)
        {
            var documentFilterContext = new OpenApiDocumentFilterContext();
            foreach (var documentFilter in documentFilters)
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
