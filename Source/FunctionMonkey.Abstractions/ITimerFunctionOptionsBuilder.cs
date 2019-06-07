using System;
using System.Collections.Generic;
using System.Text;
using FunctionMonkey.Abstractions.Builders;

namespace FunctionMonkey.Abstractions
{
    public interface ITimerFunctionOptionsBuilder : IFunctionBuilder
    {
        IOutputBindingBuilder<IFunctionBuilder> OutputTo { get; }
    }
}
