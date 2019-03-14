using FluentValidation;
using FunctionMonkey.Tests.Integration.Commands;

namespace FunctionMonkey.Tests.Integration.Validators
{
    class ResponseHandlerCommandWithResultAndValidatorThatFailsValidator : AbstractValidator<ResponseHandlerCommandWithResultAndValidatorThatFails>
    {
        public ResponseHandlerCommandWithResultAndValidatorThatFailsValidator()
        {
            RuleFor(m => m.Value).NotEqual(0);
        }
    }
}
