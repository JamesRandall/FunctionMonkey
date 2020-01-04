using FluentValidation;
using FunctionMonkey.Tests.Integration.Common.Commands.HttpResponseShaping;

namespace FunctionMonkey.Tests.Integration.Common.Validators
{
    public class HttpCommandWithNoResultAndValidatorThatFailsValidator : AbstractValidator<HttpCommandWithNoResultAndValidatorThatFails>
    {
        public HttpCommandWithNoResultAndValidatorThatFailsValidator()
        {
            RuleFor(m => m.Value).NotEqual(0);
        }
    }
}
