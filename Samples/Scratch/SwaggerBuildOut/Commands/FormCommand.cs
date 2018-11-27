using AzureFromTheTrenches.Commanding.Abstractions;
using Microsoft.AspNetCore.Http;

namespace SwaggerBuildOut.Commands
{
    public class FormCommand : ICommand
    {
        public IFormFileCollection FormFiles { get; set; }
    }
}