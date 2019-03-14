using FluentValidation;
using FunctionMonkey.Tests.Integration.Functions.Commands;

namespace FunctionMonkey.Tests.Integration.Functions.Validators
{
    public class HttpCommandWithResultAndValidatorThatPassesValidator : AbstractValidator<HttpCommandWithResultAndValidatorThatPasses>
    {
        public HttpCommandWithResultAndValidatorThatPassesValidator()
        {
            RuleFor(m => m.Value).Equal(0);
        }
    }
}
