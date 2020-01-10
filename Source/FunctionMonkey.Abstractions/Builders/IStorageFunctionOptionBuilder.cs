using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Abstractions.Builders
{
    public interface IStorageFunctionOptionBuilder<TCommand> :
        IStorageFunctionBuilder,
        IFunctionOptions<TCommand, IStorageFunctionOptionBuilder<TCommand>, IFunctionOptionsBuilder>
    {
        
    }
}