using System;
using System.IO;
using FunctionMonkey.Abstractions.Builders.Model;
using FunctionMonkey.Compiler.Implementation;
using HandlebarsDotNet;

namespace FunctionMonkey.Compiler.HandlebarsHelpers
{
    internal class OutputBindingHelperCommon
    {
        public static void WriteTemplate(TextWriter writer, AbstractFunctionDefinition functionDefinition)
        {
            TemplateProvider templateProvider = new TemplateProvider();
            string templateSource = templateProvider.GetCSharpOutputTemplate(functionDefinition.OutputBinding);
            Func<object, string> template = Handlebars.Compile(templateSource);

            string output = template(functionDefinition.OutputBinding);
            writer.Write(output);
        }
    }
}