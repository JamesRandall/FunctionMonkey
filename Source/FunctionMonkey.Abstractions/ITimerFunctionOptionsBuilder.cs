using System;
using System.Collections.Generic;
using System.Text;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions.Builders;

namespace FunctionMonkey.Abstractions
{
    public interface ITimerFunctionOptionsBuilder<TCommand> : IFunctionBuilder where TCommand : ICommand
    {
        IOutputBindingBuilder<TCommand, IFunctionBuilder> OutputTo { get; }
    }
}
