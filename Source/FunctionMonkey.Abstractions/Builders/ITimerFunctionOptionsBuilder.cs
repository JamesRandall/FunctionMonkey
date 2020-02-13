namespace FunctionMonkey.Abstractions.Builders
{
    public interface ITimerFunctionOptionsBuilder<TCommand> : IFunctionBuilder
    {
        IOutputBindingBuilder<IFunctionBuilder> OutputTo { get; }
    }
}
