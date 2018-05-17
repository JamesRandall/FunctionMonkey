namespace FunctionMonkey.Compiler.HandlebarsHelpers
{
    internal static class HandlebarsHelperRegistration
    {
        public static void RegisterHelpers()
        {
            AzureAuthenticationTypeHelper.Register();
            HttpVerbsHelper.Register();
        }
    }
}
