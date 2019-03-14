using FluentValidation;
using FunctionMonkey.Tests.Integration.Functions.Commands;

namespace FunctionMonkey.Tests.Integration.Functions.Validators
{
    public class HttpResponseHandlerCommandWithResultAndValidatorThatFailsValidator : AbstractValidator<HttpResponseHandlerCommandWithResultAndValidatorThatFails>
    {
        public HttpResponseHandlerCommandWithResultAndValidatorThatFailsValidator()
        {
            RuleFor(m => m.Value).NotEqual(0);
        }
    }
}
