namespace FunctionMonkey.Abstractions.Builders
{
    /// <summary>
    /// Allows additional function options to be configured
    /// </summary>
    public interface IFunctionOptions
    {
        IFunctionOptions JsonSerializerSettingsProvider<TJsonSerializerSettingsProvider>()
            where TJsonSerializerSettingsProvider : IJsonSerializerSettingsProvider;
    }
}