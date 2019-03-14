using FluentValidation;
using FunctionMonkey.Tests.Integration.Functions.Commands;

namespace FunctionMonkey.Tests.Integration.Functions.Validators
{
    public class HttpResponseHandlerCommandWithResultAndValidatorThatPassesValidator : AbstractValidator<HttpResponseHandlerCommandWithResultAndValidatorThatPasses>
    {
        public HttpResponseHandlerCommandWithResultAndValidatorThatPassesValidator()
        {
            RuleFor(m => m.Value).Equal(0);
        }
    }
}
