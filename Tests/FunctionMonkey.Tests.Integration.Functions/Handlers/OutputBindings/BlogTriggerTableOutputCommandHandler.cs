using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands.OutputBindings;
using FunctionMonkey.Tests.Integration.Functions.Model;

namespace FunctionMonkey.Tests.Integration.Functions.Handlers.OutputBindings
{
    class BlogTriggerTableOutputCommandHandler : ICommandHandler<BlogTriggerTableOutputCommand, MarkerTableEntity>
    {
        public Task<MarkerTableEntity> ExecuteAsync(BlogTriggerTableOutputCommand command, MarkerTableEntity previousResult)
        {
            return MarkerTableEntity.Success(command.MarkerId);
        }
    }
}
