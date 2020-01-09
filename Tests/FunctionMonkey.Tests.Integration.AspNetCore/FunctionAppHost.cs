using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Compiler.Core;
using FunctionMonkey.Tests.Integration.Common;
using FunctionMonkey.Tests.Integration.Functions;

namespace FunctionMonkey.Tests.Integration.AspNetCore
{
    public class FunctionAppHost : IFunctionAppHost
    {
        public void Build(IFunctionAppHostBuilder builder)
        {
            builder
                .CompilerOptions(options => options
                    .HttpTarget(CompileTargetEnum.AspNetCore)
                    .OutputSourceTo(@"/Users/jamesrandall/code/authoredSource")
                )
                .UseFunctionAppConfiguration<HttpFunctionAppConfiguration>();
        }
    }
}