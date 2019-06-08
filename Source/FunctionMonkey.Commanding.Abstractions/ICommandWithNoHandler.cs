using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Commanding.Abstractions
{
    /// <summary>
    /// This interface can be used to signify that a command has no handler and therefore shouldn't be dispatched.
    /// This is useful if you want to accept and validate a payload and route it directly to an output binding
    /// </summary>
    public interface ICommandWithNoHandler : ICommand
    {
        
    }
}