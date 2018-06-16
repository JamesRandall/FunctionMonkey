using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Commanding.Abstractions.Validation;

namespace FunctionMonkey.Abstractions.Validation
{
    /// <summary>
    /// Serves as an abstraction between Function Monkey and a validation framework. See Validator.cs in FunctionMonkey.FluentValidation
    /// </summary>
    public interface IValidator
    {
        ValidationResult Validate<TCommand>(TCommand command) where TCommand : ICommand;
    }
}
