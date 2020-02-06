namespace FunctionMonkey.Compiler.MSBuild
{
    public class MSBuildErrorItem
    {
        public static class SeverityLevel
        {
            public const int Error = 0;
            public const int Warning = 1;
            public const int Message = 2;
        }
        
        public int Severity { get; set; }
            
        public string Message { get; set; }
    }
}