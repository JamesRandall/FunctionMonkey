using System.Collections.Generic;

namespace FunctionMonkey.Abstractions.Builders.Model
{
    public class HeaderBindingConfiguration
    {
        /// <summary>
        /// Allows header names to be mapped to more cleanly named properties (e.g. the property OperationId is sourced from x-operation-id)
        /// The key is the property name, the value the header.
        /// </summary>
        public Dictionary<string, string> PropertyFromHeaderMappings { get; set; } = new Dictionary<string, string>();        
    }
}
