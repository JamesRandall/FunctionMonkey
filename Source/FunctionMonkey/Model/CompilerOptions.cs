using FunctionMonkey.Compiler.Core;

namespace FunctionMonkey.Model
{
    public class CompilerOptions
    {
        public CompileTargetEnum HttpTarget { get; set; } = CompileTargetEnum.AzureFunctions;
    }
}