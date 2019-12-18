using System;
using System.Linq;
using AzureFromTheTrenches.Commanding.Abstractions;
using FluentValidation;
using FunctionMonkey.Abstractions.Validation;
using FunctionMonkey.Commanding.Abstractions.Validation;
using Microsoft.Extensions.DependencyInjection;
using IValidator = FluentValidation.IValidator;

namespace FunctionMonkey.FluentValidation.Implementation
{
    internal class Validator : Abstractions.Validation.IValidator
    {
        private readonly IServiceProvider _serviceProvider;

        public Validator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ValidationResult Validate(object command)
        {
            Type validatorGenericType = typeof(IValidator<>);
            Type validatorType = validatorGenericType.MakeGenericType(command.GetType());
            
            IValidator validator = (IValidator)_serviceProvider.GetService(validatorType);
            if (validator == null)
            {
                return new ValidationResult();
            }

            var validationResult = validator.Validate(command);

            return new ValidationResult
            {
                Errors = validationResult.Errors.Select(x => new ValidationError
                {
                    ErrorCode = x.ErrorCode,
                    Message = x.ErrorMessage,
                    Property = x.PropertyName
                }).ToArray()
            };
        }
    }
}
