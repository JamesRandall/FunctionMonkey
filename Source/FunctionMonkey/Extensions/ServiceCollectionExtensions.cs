using Microsoft.Extensions.DependencyInjection;

namespace FunctionMonkey.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFunctionMonkeyClaimsPrincipal(this IServiceCollection services)
        {
            //services.AddTransient(s => FunctionMonkey.Runtime.FunctionClaimsPrincipal.Value);
            return services;
        }
    }
}
