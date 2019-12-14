using System;
using System.Collections.Generic;
using System.IO;
using FunctionMonkey.Abstractions.Builders.Model;
using FunctionMonkey.Compiler.Core.HandlebarsHelpers;
using FunctionMonkey.Model;
using HandlebarsDotNet;

namespace FunctionMonkey.Compiler.Core.Implementation
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

                if (functionDefinition is CosmosDbFunctionDefinition cosmosDbFunctionDefinition)
                {
                    if (cosmosDbFunctionDefinition.TrackRemainingWork)
                    {
                        TimerFunctionDefinition cosmosMonitorDefinition = new TimerFunctionDefinition(functionDefinition.CommandType)
                        {
                            AssemblyName = cosmosDbFunctionDefinition.AssemblyName,
                            CommandDeserializerType = null,
                            CommandType = null,
                            CronExpression = cosmosDbFunctionDefinition.RemainingWorkCronExpression,
                            FunctionClassTypeName = $"{functionDefinition.Namespace}.Monitor{functionDefinition.Name}"
                        };
                        string timerTemplateSource = _templateProvider.GetJsonTemplate(cosmosMonitorDefinition);
                        Func<object, string> timerTemplate = Handlebars.Compile(timerTemplateSource);

                        string timerJson = timerTemplate(cosmosMonitorDefinition);
                        WriteFunctionTemplate(outputBinaryFolder, $"Monitor{functionDefinition.Name}", timerJson);
                    }
                }
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
