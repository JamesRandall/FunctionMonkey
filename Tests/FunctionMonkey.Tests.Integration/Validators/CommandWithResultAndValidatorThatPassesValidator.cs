using FluentValidation;
using FunctionMonkey.Tests.Integration.Commands;

namespace FunctionMonkey.Tests.Integration.Validators
{
    class CommandWithResultAndValidatorThatPassesValidator : AbstractValidator<CommandWithResultAndValidatorThatPasses>
    {
        public CommandWithResultAndValidatorThatPassesValidator()
        {
            RuleFor(m => m.Value).Equal(0);
        }
    }
}
