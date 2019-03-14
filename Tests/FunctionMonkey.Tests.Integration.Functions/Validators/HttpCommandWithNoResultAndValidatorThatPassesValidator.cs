using FluentValidation;
using FunctionMonkey.Tests.Integration.Functions.Commands;

namespace FunctionMonkey.Tests.Integration.Functions.Validators
{
    public class HttpCommandWithNoResultAndValidatorThatPassesValidator : AbstractValidator<HttpCommandWithNoResultAndValidatorThatPasses>
    {
        public HttpCommandWithNoResultAndValidatorThatPassesValidator()
        {
            RuleFor(m => m.Value).Equal(0);
        }
    }
}
