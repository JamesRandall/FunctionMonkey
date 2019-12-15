using System;
using FunctionMonkey.Abstractions.Extensions;

namespace FunctionMonkey.Model
{
    public class ImmutableTypeConstructorParameter
    {
        public string Name { get; set; }
        
        public Type Type { get; set; }

        public string TypeName => Type?.EvaluateType();
        
        public bool IsFSharpOptionType { get; set; }
        
        public string FSharpOptionInnerTypeName { get; set; }
        
        public bool FSharpOptionInnerTypeIsString { get; set; }
    }
}