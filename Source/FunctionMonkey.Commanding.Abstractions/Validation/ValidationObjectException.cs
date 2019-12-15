using System;

namespace FunctionMonkey.Commanding.Abstractions.Validation
{
    /// <summary>
    /// An exception that is thrown out of functions that have no realtime return to another system, for example if a queue function is triggered
    /// by a queue item and that item does not conform to the expected validation rules this exception will be thrown so it can at least be logged.
    /// In contrast HTTP triggers will return a BadRequest with the ValidationResult serialized in the response.
    /// </summary>
    public class ValidationObjectException : Exception
    {
        /// <summary>
        /// Constructor for the exception
        /// </summary>
        /// <param name="functionName">The name of the function that encountered the validation errors</param>
        /// <param name="validationResult">The validation result</param>
        /// <param name="associatedId">Any associated ID (e.g. a message ID)</param>
        public ValidationObjectException(string functionName, object validationResult, string associatedId=null) : base("The command contains validation errors")
        {
            FunctionName = functionName;
            ValidationResult = validationResult;
            AssociatedId = associatedId;
        }

        /// <summary>
        /// Name of the function that encountered the validation errors
        /// </summary>
        public string FunctionName { get; }

        /// <summary>
        /// The validation result
        /// </summary>
        public object ValidationResult { get; }

        /// <summary>
        /// An associated ID (for example a message ID), or null if none available
        /// </summary>
        public string AssociatedId { get; }
    }
}