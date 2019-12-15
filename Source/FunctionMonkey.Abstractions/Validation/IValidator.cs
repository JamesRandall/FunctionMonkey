using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Commanding.Abstractions.Validation;

namespace FunctionMonkey.Abstractions.Validation
{
    /// <summary>
    /// Serves as an abstraction between Function Monkey and a validation framework. See Validator.cs in FunctionMonkey.FluentValidation
    /// </summary>
    public interface IValidator
    {
        // This no longer uses a constrained generic type as it can result in confusing implementations - the <TCommand>
        // may not be the type of the command that requires validation (it could, for example, be an ICommand type itself)
        ValidationResult Validate(object command);
    }
}
