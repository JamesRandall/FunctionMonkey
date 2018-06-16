using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.FluentValidation.Implementation;

namespace FunctionMonkey.FluentValidation
{
    /// <summary>
    /// Extension methods for adding Fluent Validation as the validator subsystem
    /// </summary>
    // ReSharper disable once InconsistentNaming - interface extensions
    public static class IFunctionHostBuilderExtensions
    {
        /// <summary>
        /// Registers Fluent Validation as the validation provider. After adding simply register your
        /// validators with the IServiceCollection in Setup() as you would normally.
        /// </summary>
        /// <param name="functionHostBuilder"></param>
        /// <returns></returns>
        public static IFunctionHostBuilder AddFluentValidation(this IFunctionHostBuilder functionHostBuilder)
        {
            functionHostBuilder.AddValidator<Validator>();
            return functionHostBuilder;
        }
    }
}
