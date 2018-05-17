using System;
using System.Collections.Generic;
using System.IO;
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
            string outputBinaryFolder,
            string assemblyName)
        {

            foreach (AbstractFunctionDefinition functionDefinition in functionDefinitions)
            {
                string templateSource = _templateProvider.GetJsonTemplate(functionDefinition);
                Func<object, string> template = Handlebars.Compile(templateSource);

                functionDefinition.AssemblyName = $"{assemblyName}.dll";
                functionDefinition.FunctionClassTypeName = $"{functionDefinition.Namespace}.{functionDefinition.Name}";
                
                string json = template(functionDefinition);
                DirectoryInfo folder =
                    Directory.CreateDirectory(Path.Combine(outputBinaryFolder, "..", functionDefinition.Name));
                string filename = Path.Combine(folder.FullName, "function.json");
                using (Stream stream = new FileStream(filename, FileMode.Create))
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(json);
                }
            }
        }
    }
}
