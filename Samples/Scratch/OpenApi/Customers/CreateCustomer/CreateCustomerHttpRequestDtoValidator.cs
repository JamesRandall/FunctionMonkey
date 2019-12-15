using FluentValidation;
using OpenApi.PropertyValidators;

namespace OpenApi.Customers.CreateCustomer
{
    public class CreateCustomerHttpRequestDtoValidator : AbstractValidator<CreateCustomerHttpRequestDto>
    {
        public CreateCustomerHttpRequestDtoValidator()
        {
            // CompanyName must be filled, or GivenName and FamilyName
            RuleFor(dto => dto).Must(c =>
                !string.IsNullOrEmpty(c.CompanyName) ||
                !string.IsNullOrEmpty(c.GivenName) && !string.IsNullOrEmpty(c.FamilyName));

            GivenNamePropertyValidator.Rule(RuleFor(dto => dto.GivenName));

            FamilyNamePropertyValidator.Rule(RuleFor(dto => dto.FamilyName));

            CompanyNamePropertyValidator.Rule(RuleFor(dto => dto.CompanyName));
        }
    }
}
