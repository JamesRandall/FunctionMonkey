using FluentValidation;
using FunctionMonkey.Tests.Integration.Commands;

namespace FunctionMonkey.Tests.Integration.Validators
{
    class ResponseHandlerCommandWithNoResultAndValidatorThatPassesValidator : AbstractValidator<ResponseHandlerCommandWithNoResultAndValidatorThatPasses>
    {
        public ResponseHandlerCommandWithNoResultAndValidatorThatPassesValidator()
        {
            RuleFor(m => m.Value).Equal(0);
        }
    }
}
