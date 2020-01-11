using System;
using FunctionMonkey.Compiler.Core;

namespace FunctionMonkey.Abstractions.Builders
{
    public interface ICompilerOptionsBuilder
    {
        ICompilerOptionsBuilder MediatorTypeSafetyEnforcer<TMediatorTypeSafetyEnforcer>()
            where TMediatorTypeSafetyEnforcer : IMediatorTypeSafetyEnforcer;
        ICompilerOptionsBuilder MediatorResultTypeExtractor<TMediatorResultTypeExtractor>()
            where TMediatorResultTypeExtractor : IMediatorResultTypeExtractor;
        ICompilerOptionsBuilder HttpTarget(CompileTargetEnum target);
        ICompilerOptionsBuilder OutputSourceTo(string folder);

        ICompilerOptionsBuilder CreateClient(Action<IClientCompilerOptionsBuilder> builder);
    }
}