using FluentValidation;
using FunctionMonkey.Tests.Integration.Functions.Commands;

namespace FunctionMonkey.Tests.Integration.Functions.Validators
{
    public class HttpResponseHandlerCommandWithNoResultAndValidatorThatPassesValidator : AbstractValidator<HttpResponseHandlerCommandWithNoResultAndValidatorThatPasses>
    {
        public HttpResponseHandlerCommandWithNoResultAndValidatorThatPassesValidator()
        {
            RuleFor(m => m.Value).Equal(0);
        }
    }
}
