using FluentValidation;
using FunctionMonkey.Tests.Integration.Common.Commands.HttpResponseShaping;

namespace FunctionMonkey.Tests.Integration.Common.Validators
{
    public class HttpResponseHandlerCommandWithResultAndValidatorThatPassesValidator : AbstractValidator<HttpResponseHandlerCommandWithResultAndValidatorThatPasses>
    {
        public HttpResponseHandlerCommandWithResultAndValidatorThatPassesValidator()
        {
            RuleFor(m => m.Value).Equal(0);
        }
    }
}
