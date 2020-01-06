using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using FunctionMonkey.Compiler.Core.Extensions;
using FunctionMonkey.Model;
using HandlebarsDotNet;

namespace FunctionMonkey.Compiler.Core.HandlebarsHelpers.AspNetCore
{
    internal static class OptionalCommaBeforeHeaderMapping
    {
        public static void Register()
        {
            Handlebars.RegisterHelper("optionalCommaBeforeHeaderMapping", (writer, context, parameters) => HelperFunction(writer, context, parameters));
        }

        private static void HelperFunction(TextWriter writer, dynamic context, object[] parameters)
        {
            if (context is HttpFunctionDefinition httpFunctionDefinition)
            {
                if ((httpFunctionDefinition.QueryParametersWithoutHeaderMapping.Any() ||
                    httpFunctionDefinition.RouteParameters.Any() ||
                    (httpFunctionDefinition.CommandRequiresBody && httpFunctionDefinition.IsBodyBased)) && httpFunctionDefinition.QueryParametersWithHeaderMapping.Any())
                {
                    writer.Write(", ");
                }
            }
            else
            {
                throw new NotSupportedException("Verbs only apply to HTTP functions");
            }
        }
    }
}
