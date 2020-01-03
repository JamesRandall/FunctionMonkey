using Microsoft.AspNetCore.Authentication;

namespace FunctionMonkey.AspNetCore
{
    public static class AuthenticationBuilderExtensions
    {
        public static AuthenticationBuilder AddTokenValidation(this AuthenticationBuilder builder)
        {
            return builder.AddScheme<AuthenticationOptions, FunctionMonkeyAuthHandler>(
                TokenValidationDefaults.AuthenticationScheme, null, _ => { });
        }
    }
}