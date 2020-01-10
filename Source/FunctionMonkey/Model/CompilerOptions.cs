using System;
using FunctionMonkey.Compiler.Core;
using FunctionMonkey.Infrastructure;

namespace FunctionMonkey.Model
{
    public class CompilerOptions
    {
        public CompileTargetEnum HttpTarget { get; set; } = CompileTargetEnum.AzureFunctions;
        public string OutputSourceTo { get; set; } = null;
        public Type MediatorTypeSafetyEnforcer { get; set; } = typeof(DefaultMediatorTypeSafetyEnforcer);
    }
}