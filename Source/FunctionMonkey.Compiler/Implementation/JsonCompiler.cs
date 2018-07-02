using System;
using System.Collections.Generic;
using System.IO;
using FunctionMonkey.Compiler.HandlebarsHelpers;
using FunctionMonkey.Model;
using HandlebarsDotNet;

namespace FunctionMonkey.Compiler.Implementation
{
    internal class JsonCompiler
    {
        private readonly ITemplateProvider _templateProvider;

        public JsonCompiler(ITemplateProvider templateProvider = null)
        {
            _templateProvider = templateProvider ?? new TemplateProvider();
        }

        public void Compile(IReadOnlyCollection<AbstractFunctionDefinition> functionDefinitions,
            OpenApiOutputModel openApiOutputModel,
            string outputBinaryFolder,
            string outputNamespaceName)
        {
            HandlebarsHelperRegistration.RegisterHelpers();

            foreach (AbstractFunctionDefinition functionDefinition in functionDefinitions)
            {
                string templateSource = _templateProvider.GetJsonTemplate(functionDefinition);
                Func<object, string> template = Handlebars.Compile(templateSource);

                functionDefinition.AssemblyName = $"{outputNamespaceName}.dll";
                functionDefinition.FunctionClassTypeName = $"{functionDefinition.Namespace}.{functionDefinition.Name}";
                
                string json = template(functionDefinition);
                WriteFunctionTemplate(outputBinaryFolder, functionDefinition.Name, json);
            }

            if (openApiOutputModel != null && openApiOutputModel.IsConfiguredForUserInterface)
            {
                string templateSource = _templateProvider.GetTemplate("swaggerui", "json");
                Func<object, string> template = Handlebars.Compile(templateSource);
                string json = template(new
                {
                    AssemblyName = $"{outputNamespaceName}.dll",
                    Namespace = outputNamespaceName
                });

                WriteFunctionTemplate(outputBinaryFolder, "OpenApiProvider", json);
            }
        }

        private static void WriteFunctionTemplate(string outputBinaryFolder, string name,
            string json)
        {
            DirectoryInfo folder =
                Directory.CreateDirectory(Path.Combine(outputBinaryFolder, "..", name));
            string filename = Path.Combine(folder.FullName, "function.json");
            using (Stream stream = new FileStream(filename, FileMode.Create))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.Write(json);
            }
        }
    }
}
