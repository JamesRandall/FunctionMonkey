using AzureFromTheTrenches.Commanding.AzureFunctions.Abstractions.Contexts;

namespace AzureFromTheTrenches.Commanding.AzureFunctions.Abstractions
{
    public interface IContextProvider
    {
        ServiceBusContext ServiceBusContext { get; }
    }
}
