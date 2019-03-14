using FluentValidation;
using FunctionMonkey.Tests.Integration.Commands;

namespace FunctionMonkey.Tests.Integration.Validators
{
    class ResponseHandlerCommandWithNoResultAndValidatorThatFailsValidator : AbstractValidator<ResponseHandlerCommandWithNoResultAndValidatorThatFails>
    {
        public ResponseHandlerCommandWithNoResultAndValidatorThatFailsValidator()
        {
            RuleFor(m => m.Value).NotEqual(0);
        }
    }
}
