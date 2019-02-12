using AzureFromTheTrenches.Commanding.Abstractions;
using SwaggerBuildOut.Commands.Responses;

namespace SwaggerBuildOut.Commands
{
    public class AddCommand : ICustomCommand<AddResult>
    {
        [SecurityProperty]
        public int ValueOne { get; set; }

        public int ValueTwo { get; set; }
    }
}
