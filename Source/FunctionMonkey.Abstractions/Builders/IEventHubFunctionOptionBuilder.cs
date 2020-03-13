using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Abstractions.Builders
{
    public interface IEventHubFunctionOptionBuilder<TCommand> : 
        IEventHubFunctionBuilder,
        IFunctionOptions<IEventHubFunctionOptionBuilder<TCommand>, IFunctionOptionsBuilder>
    {
        
    }
}