using FunctionMonkey.Abstractions.Builders.Model;
using FunctionMonkey.Model;
using HandlebarsDotNet;

namespace FunctionMonkey.Compiler.Core.HandlebarsHelpers.AspNetCore
{
    internal static class MappedHeaderNameForPropertyHelper
    {
        public static void Register()
        {
            Handlebars.RegisterHelper("mappedHeaderNameForProperty", (writer, context, parameters) =>
            {
                if (parameters[0] is HeaderBindingConfiguration headerConfig && context is HttpParameter httpParameter)
                {
                    if (headerConfig.PropertyFromHeaderMappings.TryGetValue(httpParameter.Name, out string headerName))
                    {
                        writer.Write(headerName);
                    }
                }
            });
        }
    }
}
