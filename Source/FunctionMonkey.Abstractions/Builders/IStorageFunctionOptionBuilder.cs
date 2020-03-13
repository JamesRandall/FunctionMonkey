using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Abstractions.Builders
{
    public interface IStorageFunctionOptionBuilder<TCommand> :
        IStorageFunctionBuilder,
        IFunctionOptions<IStorageFunctionOptionBuilder<TCommand>, IFunctionOptionsBuilder>
    {
        
    }
}