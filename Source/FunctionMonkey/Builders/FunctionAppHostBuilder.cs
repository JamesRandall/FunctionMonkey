using System;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Model;

namespace FunctionMonkey.Builders
{
    internal class FunctionAppHostBuilder : IFunctionAppHostBuilder
    {
        public CompilerOptions Options { get; } = new CompilerOptions();
        
        public Type FunctionAppConfiguration { get; set; }
        
        public IFunctionAppHostBuilder CompilerOptions(Action<ICompilerOptionsBuilder> options)
        {
            options(new CompilerOptionsBuilder(Options));
            return this;
        }

        public IFunctionAppHostBuilder UseFunctionAppConfiguration<TConfiguration>() where TConfiguration : IFunctionAppConfiguration
        {
            FunctionAppConfiguration = typeof(TConfiguration);
            return this;
        }
    }
}