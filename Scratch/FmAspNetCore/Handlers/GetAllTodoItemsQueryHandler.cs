using System.Collections.Generic;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FmAspNetCore.Commands;
using FmAspNetCore.Model;
using FmAspNetCore.Services;

namespace FmAspNetCore.Handlers
{
    internal class GetAllTodoItemsQueryHandler : ICommandHandler<GetAllTodoItemsQuery, IReadOnlyCollection<TodoItem>>
    {
        private readonly IRepository _repository;

        public GetAllTodoItemsQueryHandler(IRepository repository)
        {
            _repository = repository;
        }
        
        public Task<IReadOnlyCollection<TodoItem>> ExecuteAsync(GetAllTodoItemsQuery command, IReadOnlyCollection<TodoItem> previousResult)
        {
            return _repository.GetAll();
        }
    }
}