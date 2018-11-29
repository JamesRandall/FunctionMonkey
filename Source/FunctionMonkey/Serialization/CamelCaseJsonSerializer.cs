using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FunctionMonkey.Serialization
{
    /// <summary>
    /// Camel case JSON serializer
    /// </summary>
    public class CamelCaseJsonSerializer : NamingStrategyJsonSerializer
    {
        
        /// <summary>
        /// Constructor
        /// </summary>
        public CamelCaseJsonSerializer() : base(new CamelCaseNamingStrategy())
        {
            
        }
    }
}
