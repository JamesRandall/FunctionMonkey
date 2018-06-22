using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using ServiceBusSample.Commands;

namespace ServiceBusSample.Handlers
{
    internal class NewUserRegistrationCommandHandler : ICommandHandler<NewUserRegistrationCommand>
    {
        public Task ExecuteAsync(NewUserRegistrationCommand command)
        {
            // We're not really going to do anything here
            return Task.CompletedTask;
        }
    }
}
