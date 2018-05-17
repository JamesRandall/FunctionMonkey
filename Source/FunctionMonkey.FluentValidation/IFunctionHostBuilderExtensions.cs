using AzureFromTheTrenches.Commanding.AzureFunctions.Abstractions.Builders;
using AzureFromTheTrenches.Commanding.AzureFunctions.FluentValidation.Implementation;

namespace AzureFromTheTrenches.Commanding.AzureFunctions.FluentValidation
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
