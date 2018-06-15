using FluentValidation;
using GettingStartedSample.Commands;

namespace GettingStartedSample.Validators
{
    internal class HelloWorldCommandValidator : AbstractValidator<HelloWorldCommand>
    {
        public HelloWorldCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MinimumLength(1).MaximumLength(50);
        }
    }
}
