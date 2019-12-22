using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FmAspNetCore.Commands;
using FmAspNetCore.Services;

namespace FmAspNetCore.Handlers
{
    internal class MarkCompleteCommandHandler : ICommandHandler<MarkCompleteCommand>
    {
        private readonly IRepository _repository;

        public MarkCompleteCommandHandler(IRepository repository)
        {
            _repository = repository;
        }
        
        public Task ExecuteAsync(MarkCompleteCommand command)
        {
            return _repository.MarkComplete(command.Id);
        }
    }
}