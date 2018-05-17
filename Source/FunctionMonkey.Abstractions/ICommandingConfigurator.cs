using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Abstractions
{
    public interface ICommandingConfigurator
    {
        ICommandRegistry AddCommanding(ICommandingDependencyResolverAdapter adapter);
    }
}
