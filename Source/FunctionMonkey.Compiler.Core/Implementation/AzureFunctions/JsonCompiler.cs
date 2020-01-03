using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FunctionMonkey.Abstractions.Builders.Model;
using FunctionMonkey.Compiler.Core.HandlebarsHelpers.AzureFunctions;
using FunctionMonkey.Compiler.Core.Implementation.OpenApi;
using FunctionMonkey.Model;
using HandlebarsDotNet;
using Newtonsoft.Json;

namespace FunctionMonkey.Compiler.Core.Implementation.AzureFunctions
{
    internal class JsonCompiler
    {
        private readonly ITemplateProvider _templateProvider;

        public JsonCompiler(ITemplateProvider templateProvider = null)
        {
            _templateProvider = templateProvider ?? new TemplateProvider(CompileTargetEnum.AzureFunctions);
        }

        public void Compile(IReadOnlyCollection<AbstractFunctionDefinition> functionDefinitions,
            OpenApiOutputModel openApiOutputModel,
            string outputBinaryFolder,
            string outputNamespaceName)
        {
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
                    AssemblyFilename = $"{outputNamespaceName}.dll",
                    Namespace = outputNamespaceName
                });

                WriteFunctionTemplate(outputBinaryFolder, "OpenApiProvider", json);
            }

            {
                string templateSource = _templateProvider.GetTemplate("extensions", "json");
                Func<object, string> template = Handlebars.Compile(templateSource);
                string json = template(new
                {
                    AssemblyName = $"{outputNamespaceName}",
                    Namespace = outputNamespaceName
                });

                WriteExtensionsTemplate(outputBinaryFolder, json);
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

        private static void WriteExtensionsTemplate(string outputBinaryFolder,
           string json)
        {
            var functionsEntry = JsonConvert.DeserializeObject<ExtensionsJsonEntry>(json);

            string filename = Path.Combine(outputBinaryFolder, "extensions.json");

            ExtensionsJson extensions = null;
            try
            {
                using (Stream stream = new FileStream(filename, FileMode.Open))
                using (StreamReader reader = new StreamReader(stream))
                {
                    extensions = JsonConvert.DeserializeObject<ExtensionsJson>(reader.ReadToEnd());
                }
            }
            catch (FileNotFoundException)
            {
            }

            if (extensions?.Extensions?.Any(x => x.Name == functionsEntry.Name && x.Typename == functionsEntry.Typename) ?? false)
            {
                return;
            }

            extensions = ExtensionsJson.Build(extensions?.Extensions?.Append(functionsEntry)?.ToArray() ?? new[] { functionsEntry });

            using (Stream stream = new FileStream(filename, FileMode.Create))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.Write(JsonConvert.SerializeObject(extensions));
            }
        }

        private class ExtensionsJson
        {
            [JsonProperty("extensions")]
            public ExtensionsJsonEntry[] Extensions { get; set; }

            public static ExtensionsJson Build(ExtensionsJsonEntry[] extensions)
            {
                return new ExtensionsJson
                {
                    Extensions = extensions
                };
            }
        }

        private class ExtensionsJsonEntry
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("typename")]
            public string Typename { get; set; }
        }
    }
}
