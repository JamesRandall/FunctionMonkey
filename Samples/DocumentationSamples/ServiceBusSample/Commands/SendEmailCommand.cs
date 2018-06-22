using AzureFromTheTrenches.Commanding.Abstractions;

namespace ServiceBusSample.Commands
{
    public class SendEmailCommand : ICommand
    {
        public string To { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }
    }
}
