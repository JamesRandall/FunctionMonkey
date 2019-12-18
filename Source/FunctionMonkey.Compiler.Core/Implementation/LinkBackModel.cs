using System.Collections.Generic;
using FunctionMonkey.Model;

namespace FunctionMonkey.Compiler.Core.Implementation
{
    internal class LinkBackModel
    {
        public string Namespace { get; set; }

        public string TypeName { get; set; }
        
        public IReadOnlyCollection<ImmutableTypeConstructorParameter> ConstructorParameters { get; set; }
        
        public string PropertyName { get; set; }
        
        public string PropertyTypeName { get; set; }

        public bool IsPropertyReference => !string.IsNullOrWhiteSpace(PropertyName);
    }
}
