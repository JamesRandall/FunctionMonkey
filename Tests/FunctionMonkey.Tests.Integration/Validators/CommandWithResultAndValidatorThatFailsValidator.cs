using FluentValidation;
using FunctionMonkey.Tests.Integration.Commands;

namespace FunctionMonkey.Tests.Integration.Validators
{
    class CommandWithResultAndValidatorThatFailsValidator : AbstractValidator<CommandWithResultAndValidatorThatFails>
    {
        public CommandWithResultAndValidatorThatFailsValidator()
        {
            RuleFor(m => m.Value).NotEqual(0);
        }
    }
}
