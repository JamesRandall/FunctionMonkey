using Newtonsoft.Json.Serialization;

namespace FunctionMonkey.Serialization
{
    /// <summary>
    /// Uses the Json.NET default naming strategy (no modification to property case)
    /// </summary>
    public class DefaultCaseJsonSerializer : NamingStrategyJsonSerializer
    {
        public DefaultCaseJsonSerializer() : base(new DefaultNamingStrategy())
        {
        }
    }
}