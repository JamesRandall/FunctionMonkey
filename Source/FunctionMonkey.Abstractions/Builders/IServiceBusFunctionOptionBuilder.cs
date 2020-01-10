using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Abstractions.Builders
{
    public interface IServiceBusFunctionOptionBuilder<TCommand> : 
        IServiceBusFunctionBuilder,
        IFunctionOptions<TCommand, IServiceBusFunctionOptionBuilder<TCommand>, IFunctionOptionsBuilder>
    {
        
    }
}