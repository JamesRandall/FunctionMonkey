using System;

namespace FunctionMonkey.Commanding.Abstractions.Validation
{
    public class ValidationException : Exception
    {
        public ValidationException(string functionName, ValidationResult validationResult, string associatedId=null) : base("The command contains validation errors")
        {
            FunctionName = functionName;
            ValidationResult = validationResult;
            AssociatedId = associatedId;
        }

        public string FunctionName { get; }

        public ValidationResult ValidationResult { get; }

        public string AssociatedId { get; }
    }
}
