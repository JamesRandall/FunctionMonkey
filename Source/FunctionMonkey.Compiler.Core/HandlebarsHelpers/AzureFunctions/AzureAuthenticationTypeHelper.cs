using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Model;
using HandlebarsDotNet;

namespace FunctionMonkey.Compiler.Core.HandlebarsHelpers.AzureFunctions
{
    internal static class AzureAuthenticationTypeHelper
    {
        public static void Register()
        {
            RegisterCSharpAuthenticationType();
            RegisterJsonAuthenticationType();
        }

        private static void RegisterCSharpAuthenticationType()
        {
            Handlebars.RegisterHelper("azureAuthenticationType", (writer, context, parameters) =>
            {
                if (context is HttpFunctionDefinition httpFunctionDefinition)
                {
                    if (!httpFunctionDefinition.Authorization.HasValue)
                    {
                        writer.Write("AuthorizationLevel.Function");
                    }
                    else
                    {
                        // if we're using token validation then we use an anonymous function, it's secured internally with the token support
                        if (httpFunctionDefinition.Authorization == AuthorizationTypeEnum.Anonymous ||
                            httpFunctionDefinition.Authorization == AuthorizationTypeEnum.TokenValidation)
                        {
                            writer.Write("AuthorizationLevel.Anonymous");
                        }
                        else if (httpFunctionDefinition.Authorization == AuthorizationTypeEnum.Function)
                        {
                            writer.Write("AuthorizationLevel.Function");
                        }
                        else
                        {
                            throw new CompilerException(
                                $"Authorization type {httpFunctionDefinition.Authorization} can not be translated to a HTTP function");
                        }
                    }
                }
                else
                {
                    throw new CompilerException(
                        "azureAuthenticationType helper can only be used with a HttpFunctionDefinition");
                }
            });
        }

        private static void RegisterJsonAuthenticationType()
        {
            Handlebars.RegisterHelper("jsonAuthenticationType", (writer, context, parameters) =>
            {
                if (context is HttpFunctionDefinition httpFunctionDefinition)
                {
                    if (!httpFunctionDefinition.Authorization.HasValue)
                    {
                        writer.Write("function");
                    }
                    else
                    {
                        // if we're using token validation then we use an anonymous function, it's secured internally with the token support
                        if (httpFunctionDefinition.Authorization == AuthorizationTypeEnum.Anonymous ||
                            httpFunctionDefinition.Authorization == AuthorizationTypeEnum.TokenValidation)
                        {
                            writer.Write("anonymous");
                        }
                        else if (httpFunctionDefinition.Authorization == AuthorizationTypeEnum.Function)
                        {
                            writer.Write("function");
                        }
                        else
                        {
                            throw new CompilerException(
                                $"Authorization type {httpFunctionDefinition.Authorization} can not be translated to a HTTP function");
                        }
                    }
                }
                else
                {
                    throw new CompilerException(
                        "azureAuthenticationType helper can only be used with a HttpFunctionDefinition");
                }
            });
        }
    }
}
