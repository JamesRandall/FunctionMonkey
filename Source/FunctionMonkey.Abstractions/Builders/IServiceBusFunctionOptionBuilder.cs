using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Abstractions.Builders
{
    public interface IServiceBusFunctionOptionBuilder<TCommand> : 
        IServiceBusFunctionBuilder,
        IFunctionOptions<IServiceBusFunctionOptionBuilder<TCommand>, IFunctionOptionsBuilder>
    {
        
    }
}