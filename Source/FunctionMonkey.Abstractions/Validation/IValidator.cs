using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Abstractions.Validation
{
    public interface IValidator
    {
        ValidationResult Validate<TCommand>(TCommand command) where TCommand : ICommand;
    }
}
