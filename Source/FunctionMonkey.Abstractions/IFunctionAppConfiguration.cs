using FunctionMonkey.Abstractions.Builders;

namespace FunctionMonkey.Abstractions
{
    public interface IFunctionAppConfiguration
    {
        void Build(IFunctionHostBuilder builder);
    }
}
