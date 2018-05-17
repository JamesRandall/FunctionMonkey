namespace FunctionMonkey.Abstractions.Validation
{
    public class ValidationError
    {
        public SeverityEnum Severity { get; set; }

        public string ErrorCode { get; set; }

        public string Property { get; set; }

        public string Message { get; set; }
    }
}
