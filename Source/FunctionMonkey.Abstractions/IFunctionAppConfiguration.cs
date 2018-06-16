using FunctionMonkey.Abstractions.Builders;

namespace FunctionMonkey.Abstractions
{
    /// <summary>
    /// Must be implemented by a class in the Function app to configure the Azure Functions and the runtime
    /// </summary>
    public interface IFunctionAppConfiguration
    {
        /// <summary>
        /// Will be called by the compiler and runtime to configure the functions. Use the builder to create as appropriate.
        /// </summary>
        void Build(IFunctionHostBuilder builder);
    }
}
