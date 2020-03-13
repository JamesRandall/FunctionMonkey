namespace FunctionMonkey.Abstractions
{
    /// <summary>
    /// If an output converter is registered it will be called before the result of executing a command
    /// is passed into an output collector allowing the result to be shaped into a different result.
    ///
    /// This can be useful to keep protocol concerns out of the business domain or to help migrate an
    /// existing business domain into Azure Functions
    /// </summary>
    public interface IOutputBindingConverter
    {
        object Convert(object originatingCommand, object input);
    }
}