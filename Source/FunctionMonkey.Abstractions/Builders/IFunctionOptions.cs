using System;
using AzureFromTheTrenches.Commanding.Abstractions;
using Newtonsoft.Json.Serialization;

namespace FunctionMonkey.Abstractions.Builders
{
    /// <summary>
    /// Allows additional function options and overrides for defaults to be configured 
    /// </summary>
    public interface IFunctionOptions<TCommand, out TParentBuilder, out TFunctionOptionsBuilder>
        where TParentBuilder : class where TFunctionOptionsBuilder : class
    {
        TParentBuilder Options(Action<TFunctionOptionsBuilder> options);
        IOutputBindingBuilder<TCommand, TParentBuilder> OutputTo { get; }
    }
}
