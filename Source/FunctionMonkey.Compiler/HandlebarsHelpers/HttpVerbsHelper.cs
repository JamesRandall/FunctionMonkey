using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using FunctionMonkey.Model;
using HandlebarsDotNet;

namespace FunctionMonkey.Compiler.HandlebarsHelpers
{
    internal static class HttpVerbsHelper
    {
        public static void Register()
        {
            Handlebars.RegisterHelper("httpVerbs", (writer, context, parameters) => HelperFunction(writer, context, false, parameters));
            Handlebars.RegisterHelper("lowerHttpVerbs", (writer, context, parameters) => HelperFunction(writer, context, true, parameters));
        }

        private static void HelperFunction(TextWriter writer, dynamic context, bool toLowerCase, object[] parameters)
        {
            if (context is HttpFunctionDefinition httpFunctionDefinition)
            {
                HttpMethod[] verbs = httpFunctionDefinition.Verbs.ToArray();
                if (verbs.Length == 0)
                {
                    verbs = new[] {HttpMethod.Get};
                }

                StringBuilder sb = new StringBuilder($"\"{VerbToString(verbs[0], toLowerCase)}\"");

                foreach (HttpMethod verb in verbs.Skip(1))
                {
                    sb.Append(",");
                    sb.Append($"\"{VerbToString(verb, toLowerCase)}\"");
                }

                writer.Write(sb.ToString());
            }
            else
            {
                throw new CompilerException("azureAuthenticationType helper can only be used with a HttpFunctionDefinition");
            }
        }

        private static string VerbToString(HttpMethod verb, bool toLower)
        {
            return toLower ? verb.ToString().ToLower() : verb.ToString();
        }
    }
}
