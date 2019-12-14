namespace FunctionMonkey.Compiler.MSBuild
{
    public class MSBuildErrorItem
    {
        public enum SeverityEnum
        {
            Error,
            Warning,
            Message
        }

        public SeverityEnum Severity { get; set; }
            
        public string Message { get; set; }
    }
}