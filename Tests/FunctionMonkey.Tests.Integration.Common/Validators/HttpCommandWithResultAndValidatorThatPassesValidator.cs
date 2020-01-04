using FluentValidation;
using FunctionMonkey.Tests.Integration.Common.Commands.HttpResponseShaping;

namespace FunctionMonkey.Tests.Integration.Common.Validators
{
    public class HttpCommandWithResultAndValidatorThatPassesValidator : AbstractValidator<HttpCommandWithResultAndValidatorThatPasses>
    {
        public HttpCommandWithResultAndValidatorThatPassesValidator()
        {
            RuleFor(m => m.Value).Equal(0);
        }
    }
}
