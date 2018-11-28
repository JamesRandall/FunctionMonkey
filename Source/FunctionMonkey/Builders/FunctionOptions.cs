using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Model;

namespace FunctionMonkey.Builders
{
    internal class FunctionOptions : IFunctionOptions
    {
        private readonly AbstractFunctionDefinition _functionDefinition;

        public FunctionOptions(AbstractFunctionDefinition functionDefinition)
        {
            _functionDefinition = functionDefinition;
        }
        
        public IFunctionOptions JsonSerializerSettingsProvider<TJsonSerializerSettingsProvider>() where TJsonSerializerSettingsProvider : IJsonSerializerSettingsProvider
        {
            _functionDefinition.JsonSerializerSettingsProviderType = typeof(TJsonSerializerSettingsProvider);
            return this;
        }
    }
}