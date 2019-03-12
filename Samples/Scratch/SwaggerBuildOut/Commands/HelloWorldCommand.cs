using System.IO;
using AzureFromTheTrenches.Commanding.Abstractions;
using SwaggerBuildOut.Commands.Responses;

namespace SwaggerBuildOut.Commands
{
    public class HelloWorldCommand : ICommand<Content>//, IStreamCommand
    {
        public Stream Stream { get; set; }

        public string Name { get; set; }

        public string HeaderName { get; set; }
    }
}
