using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.AzureFunctions.Abstractions
{
    public interface ICommandDeserializer
    {
        TCommand Deserialize<TCommand>(string json, bool enforceSecurityProperties=true) where TCommand : ICommand;
    }
}
