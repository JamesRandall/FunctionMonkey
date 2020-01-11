using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MediatRSample.Services;
using ToDo.Application.Commands;

namespace MediatRSample.Handlers
{
    internal class MarkToDoItemCompleteCommandHandler : INotificationHandler<MarkToDoItemCompleteCommand>
    {
        private readonly IRepository _repository;

        public MarkToDoItemCompleteCommandHandler(IRepository repository)
        {
            _repository = repository;
        }
        
        public Task Handle(MarkToDoItemCompleteCommand notification, CancellationToken cancellationToken)
        {
            return _repository.MarkComplete(notification.Id);
        }
    }
}