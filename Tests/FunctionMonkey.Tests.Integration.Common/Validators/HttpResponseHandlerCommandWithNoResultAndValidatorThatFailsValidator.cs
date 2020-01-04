using FluentValidation;
using FunctionMonkey.Tests.Integration.Common.Commands.HttpResponseShaping;

namespace FunctionMonkey.Tests.Integration.Common.Validators
{
    public class HttpResponseHandlerCommandWithNoResultAndValidatorThatFailsValidator : AbstractValidator<HttpResponseHandlerCommandWithNoResultAndValidatorThatFails>
    {
        public HttpResponseHandlerCommandWithNoResultAndValidatorThatFailsValidator()
        {
            RuleFor(m => m.Value).NotEqual(0);
        }
    }
}
