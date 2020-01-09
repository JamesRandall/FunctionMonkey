using System;

namespace FunctionMonkey.Abstractions.Builders
{
    public interface IFunctionAppHostBuilder
    {
        IFunctionAppHostBuilder CompilerOptions(Action<ICompilerOptionsBuilder> options);

        IFunctionAppHostBuilder UseFunctionAppConfiguration<TConfiguration>()
            where TConfiguration : IFunctionAppConfiguration;
    }
}