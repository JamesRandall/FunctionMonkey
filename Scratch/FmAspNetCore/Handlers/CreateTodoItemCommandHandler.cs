using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FmAspNetCore.Commands;
using FmAspNetCore.Model;

namespace FmAspNetCore.Handlers
{
    internal class CreateTodoItemCommandHandler : ICommandHandler<CreateTodoItemCommand, TodoItem>
    {
        public Task<TodoItem> ExecuteAsync(CreateTodoItemCommand command, TodoItem previousResult)
        {
            return Task.FromResult(new TodoItem()
                {
                    Title = command.Title
                }
            );
        }
    }
}