using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionMonkey.Abstractions.Builders.Model
{
    public enum HeaderBindingPrecedenceEnum
    {
        BeforeQueryString = 0,
        AfterQueryString = 1
    }

    public class HeaderBindingConfiguration
    {
        /// <summary>
        /// Allows header names to be mapped to more cleanly named properties (e.g. x-operation-id to OperationId)
        /// </summary>
        public Dictionary<string, string> HeaderNamePropertyNameMappings { get; set; } = new Dictionary<string, string>();

        public HeaderBindingPrecedenceEnum HeaderBindingPrecedence { get; set; } =
            HeaderBindingPrecedenceEnum.AfterQueryString;
    }
}
