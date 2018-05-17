using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.AzureFunctions.Abstractions.Validation
{
    public interface IValidator
    {
        ValidationResult Validate<TCommand>(TCommand command) where TCommand : ICommand;
    }
}
