using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using SwaggerBuildOut.Commands;

namespace SwaggerBuildOut.Handlers
{
    internal class AddCommandHandler : ICommandHandler<AddCommand, int>
    {
        public Task<int> ExecuteAsync(AddCommand command, int previousResult)
        {
            return Task.FromResult(command.ValueOne + command.ValueTwo);
        }
    }
}
