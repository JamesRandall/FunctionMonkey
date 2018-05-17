using System;
using System.Linq;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.AzureFunctions.Abstractions.Validation;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using IValidator = AzureFromTheTrenches.Commanding.AzureFunctions.Abstractions.Validation.IValidator;

namespace AzureFromTheTrenches.Commanding.AzureFunctions.FluentValidation.Implementation
{
    internal class Validator : IValidator
    {
        private readonly IServiceProvider _serviceProvider;

        public Validator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ValidationResult Validate<TCommand>(TCommand command) where TCommand : ICommand
        {
            IValidator<TCommand> validator = _serviceProvider.GetService<IValidator<TCommand>>();
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
