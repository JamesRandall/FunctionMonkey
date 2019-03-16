using System;
using FunctionMonkey.Commanding.Abstractions.Validation;

namespace FunctionMonkey.Testing
{
    public class ValidationException : Exception
    {
        public ValidationException(ValidationResult validationResult)
        {
            ValidationResult = validationResult;
        }

        public ValidationResult ValidationResult { get; }
    }
}
