using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Abstractions
{
    /// <summary>
    /// Can be implemented by the class that implements IFunctionAppConfiguration to customize how the AzureFromTheTrenches.Commanding system
    /// is initialised.
    /// </summary>
    public interface ICommandingConfigurator
    {
        /// <summary>
        /// Implementations should add the commanding system within this method. See https://commanding.azurefromthetrenches.com for details
        /// </summary>
        ICommandRegistry AddCommanding(ICommandingDependencyResolverAdapter adapter);
    }
}
