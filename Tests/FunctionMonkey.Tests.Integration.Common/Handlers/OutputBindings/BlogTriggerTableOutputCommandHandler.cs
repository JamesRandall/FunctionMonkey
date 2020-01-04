using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Common.Commands.OutputBindings;
using FunctionMonkey.Tests.Integration.Common.Model;

namespace FunctionMonkey.Tests.Integration.Common.Handlers.OutputBindings
{
    class BlogTriggerTableOutputCommandHandler : ICommandHandler<BlogTriggerTableOutputCommand, MarkerTableEntity>
    {
        public Task<MarkerTableEntity> ExecuteAsync(BlogTriggerTableOutputCommand command, MarkerTableEntity previousResult)
        {
            return MarkerTableEntity.Success(command.MarkerId);
        }
    }
}
