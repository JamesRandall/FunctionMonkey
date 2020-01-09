using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Compiler.Core;
using FunctionMonkey.Tests.Integration.Common;

namespace FunctionMonkey.Tests.Integration.Functions
{
    public class FunctionAppHost : IFunctionAppHost
    {
        public void Build(IFunctionAppHostBuilder builder)
        {
            builder
                .CompilerOptions(options => options
                    .HttpTarget(CompileTargetEnum.AzureFunctions)
                    .OutputSourceTo(@"/Users/jamesrandall/code/authoredSource")
                )
                .UseFunctionAppConfiguration<FullFunctionAppConfiguration>();
        }
    }
}