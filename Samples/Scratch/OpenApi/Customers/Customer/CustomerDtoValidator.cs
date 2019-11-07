using FluentValidation;
using OpenApi.PropertyValidators;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenApi.Customers.Customer
{
    public class CustomerDtoValidator : AbstractValidator<CustomerDto>
    {
        public CustomerDtoValidator()
        {
            // CompanyName must be filled, or GivenName and FamilyName
            RuleFor(dto => dto).Must(c =>
                !string.IsNullOrEmpty(c.CompanyName) ||
                !string.IsNullOrEmpty(c.GivenName) && !string.IsNullOrEmpty(c.FamilyName));

            IdPropertyValidator.Rule(RuleFor(dto => dto.Id).NotEmpty());

            RuleFor(dto => dto.CreatedAt)
                .NotEmpty();

            RuleFor(dto => dto.State)
                .NotEmpty();

            GivenNamePropertyValidator.Rule(RuleFor(dto => dto.GivenName));

            FamilyNamePropertyValidator.Rule(RuleFor(dto => dto.FamilyName));

            CompanyNamePropertyValidator.Rule(RuleFor(dto => dto.CompanyName));

            LanguageCodePropertyValidator.Rule(RuleFor(dto => dto.LanguageCode).NotEmpty());
        }
    }
}
