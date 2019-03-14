using FluentValidation;
using FunctionMonkey.Tests.Integration.Commands;

namespace FunctionMonkey.Tests.Integration.Validators
{
    class CommandWithNoResultAndValidatorThatFailsValidator : AbstractValidator<CommandWithNoResultAndValidatorThatFails>
    {
        public CommandWithNoResultAndValidatorThatFailsValidator()
        {
            RuleFor(m => m.Value).NotEqual(0);
        }
    }
}
