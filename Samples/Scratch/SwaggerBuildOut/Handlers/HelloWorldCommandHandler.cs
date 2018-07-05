using System.IO;
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
            using (StreamReader reader = new StreamReader(command.Stream))
            {
                string json = reader.ReadToEnd();
                return Task.FromResult(new Message
                {
                    Text = $"JSON payload\n{json}"
                });
            }                
        }
    }
}
