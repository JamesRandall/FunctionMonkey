using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using SwaggerBuildOut.Commands;

namespace SwaggerBuildOut.Handlers
{
    public class FormCommandHandler : ICommandHandler<FormCommand>
    {
        public Task ExecuteAsync(FormCommand command)
        {
            throw new System.NotImplementedException();
        }
    }
}