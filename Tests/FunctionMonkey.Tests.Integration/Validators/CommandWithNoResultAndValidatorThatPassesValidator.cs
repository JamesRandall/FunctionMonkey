using FluentValidation;
using FunctionMonkey.Tests.Integration.Commands;

namespace FunctionMonkey.Tests.Integration.Validators
{
    class CommandWithNoResultAndValidatorThatPassesValidator : AbstractValidator<CommandWithNoResultAndValidatorThatPasses>
    {
        public CommandWithNoResultAndValidatorThatPassesValidator()
        {
            RuleFor(m => m.Value).Equal(0);
        }
    }
}
