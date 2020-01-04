using FluentValidation;
using FunctionMonkey.Tests.Integration.Common.Commands.HttpResponseShaping;

namespace FunctionMonkey.Tests.Integration.Common.Validators
{
    public class HttpCommandWithResultAndValidatorThatFailsValidator : AbstractValidator<HttpCommandWithResultAndValidatorThatFails>
    {
        public HttpCommandWithResultAndValidatorThatFailsValidator()
        {
            RuleFor(m => m.Value).NotEqual(0);
        }
    }
}
