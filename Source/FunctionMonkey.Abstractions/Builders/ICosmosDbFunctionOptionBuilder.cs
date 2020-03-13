using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Abstractions.Builders
{
    public interface ICosmosDbFunctionOptionBuilder<TCommand> : ICosmosDbFunctionBuilder, IFunctionOptions<ICosmosDbFunctionOptionBuilder<TCommand>, IFunctionOptionsBuilder>
    {
        
    }
}