using System;
using System.IO;
using FunctionMonkey.Abstractions.Builders.Model;
using FunctionMonkey.Compiler.Core.Implementation;
using HandlebarsDotNet;

namespace FunctionMonkey.Compiler.Core.HandlebarsHelpers.AzureFunctions
{
    internal static class ParameterOutputBindingHelper
    {
        public static void Register()
        {
            Handlebars.RegisterHelper("parameterOutputBinding", (writer, context, parameters) => HelperFunction(writer, context));
        }

        private static void HelperFunction(TextWriter writer, dynamic context)
        {
            if (context is AbstractFunctionDefinition functionDefinition)
            {
                if (functionDefinition.OutputBinding == null)
                {
                    return;
                }

                WriteTemplate(writer, functionDefinition);
            }
        }

        private static void WriteTemplate(TextWriter writer, AbstractFunctionDefinition functionDefinition)
        {
            TemplateProvider templateProvider = new TemplateProvider(CompileTargetEnum.AzureFunctions);
            string templateSource = templateProvider.GetCSharpOutputParameterTemplate(functionDefinition.OutputBinding);
            Func<object, string> template = Handlebars.Compile(templateSource);

            string output = template(functionDefinition.OutputBinding);
            writer.Write(",");
            writer.Write(output);
        }
    }
}
