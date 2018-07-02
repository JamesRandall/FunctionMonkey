using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using SwaggerBuildOut.Commands;
using SwaggerBuildOut.Commands.Responses;

namespace SwaggerBuildOut.Handlers
{
    internal class HelloWorldCommandHandler : ICommandHandler<HelloWorldCommand, Message>
    {
        public Task<Message> ExecuteAsync(HelloWorldCommand command, Message previousResult)
        {
            return Task.FromResult(new Message
            {
                Text = $"Hello {command.Name}"
            });
        }
    }
}
