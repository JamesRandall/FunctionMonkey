using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using FunctionMonkey.Compiler.HandlebarsHelpers;
using FunctionMonkey.Model;
using HandlebarsDotNet;

namespace FunctionMonkey.Compiler.Implementation
{
    internal class ProxiesJsonCompiler
    {
        private readonly ITemplateProvider _templateProvider;        

        public ProxiesJsonCompiler(ITemplateProvider templateProvider = null)
        {
            _templateProvider = templateProvider ?? new TemplateProvider();
        }

        public void Compile(IReadOnlyCollection<AbstractFunctionDefinition> functionDefinitions,
            OpenApiConfiguration openApiConfiguration,
            OpenApiOutputModel openApiOutputModel,
            string outputBinaryFolder)
        {
            HandlebarsHelperRegistration.RegisterHelpers();

            HttpFunctionDefinition[] httpFunctionDefinitions = functionDefinitions
                .OfType<HttpFunctionDefinition>()
                .Where(x => !string.IsNullOrWhiteSpace(x.Route))
                .ToArray();
            if (!httpFunctionDefinitions.Any())
            {
                return;
            }

            List<object> proxyDefinitions = new List<object>(httpFunctionDefinitions);
            if (openApiConfiguration.IsOpenApiOutputEnabled)
            {
                proxyDefinitions.Add(new
                {
                    Name = "OpenApiYaml",
                    Route = "/openapi.yaml",
                    IsOpenApiYaml = true
                });

                Debug.Assert(openApiOutputModel != null);
                if (openApiOutputModel.IsConfiguredForUserInterface)
                {
                    proxyDefinitions.Add(new
                    {
                        Name = "OpenApiProvider",
                        Route = openApiConfiguration.UserInterfaceRoute + "/{name}",
                        IsOpenApiUi = true
                    });
                }
            }
            

            string templateSource = _templateProvider.GetProxiesJsonTemplate();
            Func<object, string> template = Handlebars.Compile(templateSource);

            string json = template(proxyDefinitions);
            DirectoryInfo folder = Directory.CreateDirectory(Path.Combine(outputBinaryFolder, ".."));
            string filename = Path.Combine(folder.FullName, "proxies.json");
            using (Stream stream = new FileStream(filename, FileMode.Create))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.Write(json);
            }
        }
    }
}
