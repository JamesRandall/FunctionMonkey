namespace FunctionMonkey.Compiler.Core.HandlebarsHelpers.AspNetCore
{
    internal static class HandlebarsHelperRegistration
    {
        public static void RegisterHelpers()
        {
            HttpVerbsHelper.Register();
            OptionalCommaBeforeHeaderMapping.Register();
            MappedHeaderNameForPropertyHelper.Register();
        }
    }
}