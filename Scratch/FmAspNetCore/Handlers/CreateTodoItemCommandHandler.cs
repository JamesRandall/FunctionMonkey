using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FmAspNetCore.Commands;
using FmAspNetCore.Model;
using FmAspNetCore.Services;

namespace FmAspNetCore.Handlers
{
    internal class CreateTodoItemCommandHandler : ICommandHandler<CreateTodoItemCommand, TodoItem>
    {
        private readonly IRepository _repository;

        public CreateTodoItemCommandHandler(IRepository repository)
        {
            _repository = repository;
        }
        
        public Task<TodoItem> ExecuteAsync(CreateTodoItemCommand command, TodoItem previousResult)
        {
            return _repository.Create(command.Title);
        }
    }
}