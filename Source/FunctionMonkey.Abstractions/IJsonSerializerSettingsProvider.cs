using Newtonsoft.Json;

namespace FunctionMonkey.Abstractions
{
    public interface IJsonSerializerSettingsProvider
    {
        JsonSerializerSettings Get();
    }
}