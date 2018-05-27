using System.Collections.Generic;

namespace FunctionMonkey.Commanding.Abstractions.Validation
{
    public class ValidationResult
    {
        public IReadOnlyCollection<ValidationError> Errors { get; set; } = new ValidationError[0];

        public bool IsValid => Errors == null || Errors.Count == 0;
    }
}
