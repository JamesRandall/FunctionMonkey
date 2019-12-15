using FluentValidation;
using OpenApi.PropertyValidators;

namespace OpenApi.Customers.GetCustomers
{
    public class GetCustomersHttpRequestDtoValidator : AbstractValidator<GetCustomersHttpRequestDto>
    {
        public GetCustomersHttpRequestDtoValidator()
        {
            IdPropertyValidator.Rule(RuleFor(dto => dto.After));

            IdPropertyValidator.Rule(RuleFor(dto => dto.Before));

            LimitPropertyValidator.Rule(RuleFor(dto => dto.Limit));
        }
    }
}
