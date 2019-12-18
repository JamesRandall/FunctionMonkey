using FluentValidation;
using OpenApi.Customers.Customer;
using OpenApi.Dtos;

namespace OpenApi.Customers.GetCustomers
{
    public class GetCustomersHttpResponseDtoValidator : AbstractValidator<GetCustomersHttpResponseDto>
    {
        public GetCustomersHttpResponseDtoValidator()
        {
            RuleFor(dto => dto.Meta)
                .SetValidator(new CursorPagedMetaDtoValidator());

            RuleForEach(dto => dto.Customers)
                .SetValidator(new CustomerDtoValidator());
        }
    }
}
