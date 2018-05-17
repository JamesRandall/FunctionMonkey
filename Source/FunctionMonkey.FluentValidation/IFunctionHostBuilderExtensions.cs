using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.FluentValidation.Implementation;

namespace FunctionMonkey.FluentValidation
{
    // ReSharper disable once InconsistentNaming - interface extensions
    public static class IFunctionHostBuilderExtensions
    {
        public static IFunctionHostBuilder AddFluentValidation(this IFunctionHostBuilder functionHostBuilder)
        {
            functionHostBuilder.AddValidator<Validator>();
            return functionHostBuilder;
        }
    }
}
