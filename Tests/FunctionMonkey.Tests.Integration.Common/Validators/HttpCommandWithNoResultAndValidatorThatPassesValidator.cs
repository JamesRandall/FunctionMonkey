using FluentValidation;
using FunctionMonkey.Tests.Integration.Common.Commands.HttpResponseShaping;

namespace FunctionMonkey.Tests.Integration.Common.Validators
{
    public class HttpCommandWithNoResultAndValidatorThatPassesValidator : AbstractValidator<HttpCommandWithNoResultAndValidatorThatPasses>
    {
        public HttpCommandWithNoResultAndValidatorThatPassesValidator()
        {
            RuleFor(m => m.Value).Equal(0);
        }
    }
}
