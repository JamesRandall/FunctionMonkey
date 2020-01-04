using FluentValidation;
using FunctionMonkey.Tests.Integration.Common.Commands.HttpResponseShaping;

namespace FunctionMonkey.Tests.Integration.Common.Validators
{
    public class HttpResponseHandlerCommandWithResultAndValidatorThatFailsValidator : AbstractValidator<HttpResponseHandlerCommandWithResultAndValidatorThatFails>
    {
        public HttpResponseHandlerCommandWithResultAndValidatorThatFailsValidator()
        {
            RuleFor(m => m.Value).NotEqual(0);
        }
    }
}
