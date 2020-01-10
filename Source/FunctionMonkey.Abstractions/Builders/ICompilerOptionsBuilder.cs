using FunctionMonkey.Compiler.Core;

namespace FunctionMonkey.Abstractions.Builders
{
    public interface ICompilerOptionsBuilder
    {
        ICompilerOptionsBuilder MediatorTypeSafetyEnforcer<TMediatorTypeSafetyEnforcer>()
            where TMediatorTypeSafetyEnforcer : IMediatorTypeSafetyEnforcer;
        ICompilerOptionsBuilder HttpTarget(CompileTargetEnum target);
        ICompilerOptionsBuilder OutputSourceTo(string folder);
    }
}