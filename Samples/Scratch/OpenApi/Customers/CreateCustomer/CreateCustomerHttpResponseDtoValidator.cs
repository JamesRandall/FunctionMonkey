using FluentValidation;
using OpenApi.Customers.Customer;

namespace OpenApi.Customers.CreateCustomer
{
    public class CreateCustomerHttpResponseDtoValidator : AbstractValidator<CreateCustomerHttpResponseDto>
    {
        public CreateCustomerHttpResponseDtoValidator()
        {
            RuleFor(dto => dto.Customer)
                .NotEmpty()
                .SetValidator(new CustomerDtoValidator());
        }
    }
}
