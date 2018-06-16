using System.Collections.Generic;

namespace FunctionMonkey.Commanding.Abstractions.Validation
{
    /// <summary>
    /// Represents a validation result
    /// </summary>
    public class ValidationResult
    {
        /// <summary>
        /// The set of errors (if any)
        /// </summary>
        public IReadOnlyCollection<ValidationError> Errors { get; set; } = new ValidationError[0];

        /// <summary>
        /// Is the result valid
        /// </summary>
        public bool IsValid => Errors == null || Errors.Count == 0;
    }

    /// <summary>
    /// Represents a validation result and associated command response
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public class ValidationResult<TResult> : ValidationResult
    {
        /// <summary>
        /// The result
        /// </summary>
        public TResult Result { get; set; }
    }
}
