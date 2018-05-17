using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AzureFromTheTrenches.Commanding.AzureFunctions.Compiler.HandlebarsHelpers;
using AzureFromTheTrenches.Commanding.AzureFunctions.Model;
using HandlebarsDotNet;

namespace AzureFromTheTrenches.Commanding.AzureFunctions.Compiler.Implementation
{
    internal class ProxiesJsonCompiler
    {
        private readonly ITemplateProvider _templateProvider;        

        public ProxiesJsonCompiler(ITemplateProvider templateProvider = null)
        {
            _templateProvider = templateProvider ?? new TemplateProvider();
        }

        public void Compile(IReadOnlyCollection<AbstractFunctionDefinition> functionDefinitions,
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

            string templateSource = _templateProvider.GetProxiesJsonTemplate();
            Func<object, string> template = Handlebars.Compile(templateSource);

            string json = template(httpFunctionDefinitions);
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
