using FunctionMonkey.Compiler.Core;

namespace FunctionMonkey.Abstractions.Builders
{
    public interface ICompilerOptionsBuilder
    {
        ICompilerOptionsBuilder HttpTarget(CompileTargetEnum target);
    }
}