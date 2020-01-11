using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MediatRSample.Models;
using MediatRSample.Services;
using ToDo.Application.Commands;

namespace MediatRSample.Handlers
{
    internal class GetAllToDoItemsQueryHandler : IRequestHandler<GetAllToDoItemsQuery, IReadOnlyCollection<ToDoItem>>
    {
        private readonly IRepository _repository;

        public GetAllToDoItemsQueryHandler(IRepository repository)
        {
            _repository = repository;
        }
        
        public Task<IReadOnlyCollection<ToDoItem>> Handle(GetAllToDoItemsQuery request, CancellationToken cancellationToken)
        {
            return _repository.GetAll();
        }
    }
}