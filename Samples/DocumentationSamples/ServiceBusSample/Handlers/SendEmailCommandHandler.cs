using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions;
using ServiceBusSample.Commands;

namespace ServiceBusSample.Handlers
{
    internal class SendEmailCommandHandler : ICommandHandler<SendEmailCommand>
    {
        private readonly IContextProvider _contextProvider;

        public SendEmailCommandHandler(IContextProvider contextProvider)
        {
            _contextProvider = contextProvider;
        }

        public Task ExecuteAsync(SendEmailCommand command)
        {
            if (_contextProvider.ServiceBusContext.DeliveryCount >= 5)
            {
                // do something else
            }
            // We won't really send an email from this sample
            return Task.CompletedTask;
        }
    }
}
