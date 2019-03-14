using FluentValidation;
using FunctionMonkey.Tests.Integration.Functions.Commands;

namespace FunctionMonkey.Tests.Integration.Functions.Validators
{
    public class HttpCommandWithNoResultAndValidatorThatFailsValidator : AbstractValidator<HttpCommandWithNoResultAndValidatorThatFails>
    {
        public HttpCommandWithNoResultAndValidatorThatFailsValidator()
        {
            RuleFor(m => m.Value).NotEqual(0);
        }
    }
}
