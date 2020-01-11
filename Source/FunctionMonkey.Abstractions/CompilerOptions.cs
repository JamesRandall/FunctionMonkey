using System;
using FunctionMonkey.Compiler.Core;

namespace FunctionMonkey.Model
{
    public class CompilerOptions
    {
        public CompileTargetEnum HttpTarget { get; set; } = CompileTargetEnum.AzureFunctions;
        public string OutputSourceTo { get; set; }
        public Type MediatorTypeSafetyEnforcer { get; set; }
        public Type MediatorResultTypeExtractor { get; set; }
        public ClientCompilerOptions Client { get; set; }
    }
}