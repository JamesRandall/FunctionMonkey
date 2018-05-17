using AzureFromTheTrenches.Commanding.AzureFunctions.Abstractions.Builders;

namespace AzureFromTheTrenches.Commanding.AzureFunctions.Abstractions
{
    public interface IFunctionAppConfiguration
    {
        void Build(IFunctionHostBuilder builder);
    }
}
