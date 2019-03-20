using System.IO;
using FunctionMonkey.Abstractions.Builders.Model;
using FunctionMonkey.Compiler.Extensions;
using FunctionMonkey.Model;
using HandlebarsDotNet;

namespace FunctionMonkey.Compiler.HandlebarsHelpers
{
    internal static class ParameterOutputBindingHelper
    {
        public static void Register()
        {
            Handlebars.RegisterHelper("parameterOutputBindingHelper", (writer, context, parameters) => HelperFunction(writer, context));
        }

        private static void HelperFunction(TextWriter writer, dynamic context)
        {
            if (context is AbstractFunctionDefinition functionDefinition)
            {
                if (functionDefinition.OutputBinding == null)
                {
                    return;
                }

                if (functionDefinition.OutputBinding.IsReturnType)
                {
                    return;
                }
                
                OutputBindingHelperCommon.WriteTemplate(writer, functionDefinition);
            }
        }
    }
}
