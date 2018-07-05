using System.IO;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Commanding.Abstractions;
using SwaggerBuildOut.Commands.Responses;

namespace SwaggerBuildOut.Commands
{
    public class HelloWorldCommand : ICommand<Message>, IStreamCommand
    {
        public Stream Stream { get; set; }

        public string Name { get; set; }
    }
}
