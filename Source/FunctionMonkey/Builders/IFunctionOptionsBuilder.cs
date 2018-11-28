using System;
using FunctionMonkey.Abstractions.Builders;

namespace FunctionMonkey.Builders
{
    public interface IFunctionOptionsBuilder<out TBuilderInterface>
    {
        TBuilderInterface Options(Action<IFunctionOptions> options);
    }
}