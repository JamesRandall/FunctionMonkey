namespace FunctionMonkey.Abstractions.Builders
{
    public interface ITimerFunctionOptionsBuilder<TCommand> : IFunctionBuilder
    {
        IOutputBindingBuilder<TCommand, IFunctionBuilder> OutputTo { get; }
    }
}
