using System;
using FunctionMonkey.Abstractions.Extensions;

namespace FunctionMonkey.Model
{
    public class ImmutableTypeConstructorParameter
    {
        public string Name { get; set; }
        
        public Type Type { get; set; }

        public string TypeName => Type?.EvaluateType();
    }
}