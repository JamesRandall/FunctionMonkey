using AzureFromTheTrenches.Commanding.Abstractions;
using MultiAssemblySample.Application.Handlers;

namespace MultiAssemblySample.Application
{
    // ReSharper disable once InconsistentNaming
    public static class ICommandRegistryExtensions
    {
        public static ICommandRegistry AddApplication(this ICommandRegistry commandRegistry)
        {
            commandRegistry.Register<SimpleCommandHandler>();
            return commandRegistry;
        }
    }
}
