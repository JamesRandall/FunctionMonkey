using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Abstractions
{
    public interface ICommandDeserializer
    {
        TCommand Deserialize<TCommand>(string json, bool enforceSecurityProperties=true) where TCommand : ICommand;
    }
}
