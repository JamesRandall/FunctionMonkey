using FluentValidation;
using FunctionMonkey.Tests.Integration.Commands;

namespace FunctionMonkey.Tests.Integration.Validators
{
    class ResponseHandlerCommandWithResultAndValidatorThatPassesValidator : AbstractValidator<ResponseHandlerCommandWithResultAndValidatorThatPasses>
    {
        public ResponseHandlerCommandWithResultAndValidatorThatPassesValidator()
        {
            RuleFor(m => m.Value).Equal(0);
        }
    }
}
