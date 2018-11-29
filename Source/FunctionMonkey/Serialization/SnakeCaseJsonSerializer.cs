using Newtonsoft.Json.Serialization;

namespace FunctionMonkey.Serialization
{
    /// <summary>
    /// A snake case JSON serializer
    /// </summary>
    public class SnakeCaseJsonSerializer : NamingStrategyJsonSerializer
    {
        public SnakeCaseJsonSerializer() : base(new SnakeCaseNamingStrategy())
        {
        }
    }
}