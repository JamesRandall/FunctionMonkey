namespace FunctionMonkey.Abstractions
{
    public interface ICommandTransformer
    {
        TCommand Transform<TCommand>(TCommand input);
    }
}