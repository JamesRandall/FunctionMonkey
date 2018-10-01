using System;
using System.IO;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using ExternalTypes;
using FunctionMonkey.Abstractions;
using SwaggerBuildOut.Commands;
using SwaggerBuildOut.Commands.Responses;

namespace SwaggerBuildOut.Handlers
{
    internal class HelloWorldCommandHandler : ICommandHandler<HelloWorldCommand, Content>
    {
        private readonly IContextProvider _contextProvider;

        public HelloWorldCommandHandler(IContextProvider contextProvider)
        {
            _contextProvider = contextProvider;
        }

        public Task<Content> ExecuteAsync(HelloWorldCommand command, Content previousResult)
        {
            if (command.Stream != null)
            {
                using (StreamReader reader = new StreamReader(command.Stream))
                {
                    string json = reader.ReadToEnd();
                    return Task.FromResult(new Content
                    {
                        Message = new Message
                        {
                            Text = $"JSON payload\n{json}"
                        }
                    });
                }
            }

            Console.WriteLine("Headers");
            foreach (var kvp in _contextProvider.HttpContext.Headers)
            {
                foreach (string value in kvp.Value)
                {
                    Console.WriteLine($"{kvp.Key}: {value}");
                }
            }

            return Task.FromResult(new Content()
            {
                Message = new Message
                {
                    Text = $"Hello {command.Name}, invocation ID: {_contextProvider.ExecutionContext.InvocationId}, x-header-name: {command.HeaderName ?? "empty" }"
                }
            });
        }
    }
}
