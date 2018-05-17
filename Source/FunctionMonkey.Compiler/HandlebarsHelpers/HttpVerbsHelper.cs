using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using AzureFromTheTrenches.Commanding.AzureFunctions.Model;
using HandlebarsDotNet;

namespace AzureFromTheTrenches.Commanding.AzureFunctions.Compiler.HandlebarsHelpers
{
    internal static class HttpVerbsHelper
    {
        public static void Register()
        {
            Handlebars.RegisterHelper("httpVerbs", (writer, context, parameters) =>
            {
                if (context is HttpFunctionDefinition httpFunctionDefinition)
                {
                    HttpMethod[] verbs = httpFunctionDefinition.Verbs.ToArray();
                    if (verbs.Length == 0)
                    {
                        verbs = new[] {HttpMethod.Get};
                    }
                    StringBuilder sb = new StringBuilder($"\"{verbs[0]}\"");

                    foreach (HttpMethod verb in verbs.Skip(1))
                    {
                        sb.Append(",");
                        sb.Append($"\"{verb}\"");
                    }
                    writer.Write(sb.ToString());
                }
                else
                {
                    throw new CompilerException("azureAuthenticationType helper can only be used with a HttpFunctionDefinition");
                }                
            });
        }
    }
}
