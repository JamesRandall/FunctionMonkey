namespace FunctionMonkey.Commanding.Abstractions.Validation
{
    /// <summary>
    /// Represents a validation error
    /// </summary>
    public class ValidationError
    {
        /// <summary>
        /// The severity of the error
        /// </summary>
        public SeverityEnum Severity { get; set; }

        /// <summary>
        /// An optional error code for the error
        /// </summary>
        public string ErrorCode { get; set; }

        /// <summary>
        /// The, optional, name of the property that the error is in relation to
        /// </summary>
        public string Property { get; set; }

        /// <summary>
        /// The, optional, validation message
        /// </summary>
        public string Message { get; set; }
    }
}
