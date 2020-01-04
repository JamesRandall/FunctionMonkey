using FluentValidation;
using FunctionMonkey.Tests.Integration.Common.Commands.HttpResponseShaping;

namespace FunctionMonkey.Tests.Integration.Common.Validators
{
    public class HttpResponseHandlerCommandWithNoResultAndValidatorThatPassesValidator : AbstractValidator<HttpResponseHandlerCommandWithNoResultAndValidatorThatPasses>
    {
        public HttpResponseHandlerCommandWithNoResultAndValidatorThatPassesValidator()
        {
            RuleFor(m => m.Value).Equal(0);
        }
    }
}
