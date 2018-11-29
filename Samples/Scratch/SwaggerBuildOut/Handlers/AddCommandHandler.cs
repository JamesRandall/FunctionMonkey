using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using SwaggerBuildOut.Commands;
using SwaggerBuildOut.Commands.Responses;

namespace SwaggerBuildOut.Handlers
{
    internal class AddCommandHandler : ICommandHandler<AddCommand, AddResult>
    {
        public Task<AddResult> ExecuteAsync(AddCommand command, AddResult previousResult)
        {
            return Task.FromResult(new AddResult
            {
                ValueResultThing = command.ValueOne + command.ValueTwo
            });
        }
    }
}
