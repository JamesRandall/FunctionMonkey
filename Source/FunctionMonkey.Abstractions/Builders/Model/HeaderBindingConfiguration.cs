using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionMonkey.Abstractions.Builders.Model
{
    public class HeaderBindingConfiguration
    {
        /// <summary>
        /// Allows header names to be mapped to more cleanly named properties (e.g. the property OperationId is sourced from x-operation-id)
        /// The key is the property name, the value the header.
        /// </summary>
        public Dictionary<string, string> PropertyFromHeaderMappings { get; set; } = new Dictionary<string, string>();
        
        /// <summary>
        /// Is header binding enabled. Defaults to false.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// If true then any headers with a matching property name will be mapped. Defaults to false.
        /// </summary>
        public bool AutoMapHeadersBasedOnName { get; set; }
    }
}
