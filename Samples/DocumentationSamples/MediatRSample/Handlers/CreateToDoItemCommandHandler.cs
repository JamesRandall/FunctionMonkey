using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MediatRSample.Models;
using MediatRSample.Services;
using ToDo.Application.Commands;

namespace MediatRSample.Handlers
{
    internal class CreateToDoItemCommandHandler : IRequestHandler<CreateToDoItemCommand, ToDoItem>
    {
        private readonly IRepository _repository;

        public CreateToDoItemCommandHandler(IRepository repository)
        {
            _repository = repository;
        }
        
        public Task<ToDoItem> Handle(CreateToDoItemCommand request, CancellationToken cancellationToken)
        {
            return _repository.Create(request.Title);
        }
    }
}