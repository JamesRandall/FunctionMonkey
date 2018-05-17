using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.AzureFunctions.Abstractions
{
    public interface ICommandingConfigurator
    {
        ICommandRegistry AddCommanding(ICommandingDependencyResolverAdapter adapter);
    }
}
