using FluentValidation;
using FunctionMonkey.Tests.Integration.Functions.Commands;

namespace FunctionMonkey.Tests.Integration.Functions.Validators
{
    public class HttpResponseHandlerCommandWithNoResultAndValidatorThatFailsValidator : AbstractValidator<HttpResponseHandlerCommandWithNoResultAndValidatorThatFails>
    {
        public HttpResponseHandlerCommandWithNoResultAndValidatorThatFailsValidator()
        {
            RuleFor(m => m.Value).NotEqual(0);
        }
    }
}
