using FunctionMonkey.Abstractions.Builders;

namespace FunctionMonkey.Abstractions
{
    public interface IFunctionAppHost
    {
        void Build(IFunctionAppHostBuilder builder);
    }
}