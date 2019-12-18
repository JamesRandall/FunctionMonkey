using System;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using RestAspNetCoreExample.Commands;
using RestAspNetCoreExample.Model;

namespace RestAspNetCoreExample.Handlers
{
    internal class CreateToDoItemCommandHandler : ICommandHandler<CreateTodoItemCommand, TodoItem>
    {
        public Task<TodoItem> ExecuteAsync(CreateTodoItemCommand command, TodoItem previousResult)
        {
            System.Console.WriteLine("creating todo item");
            return Task.FromResult(new TodoItem()
            {
                Id = Guid.NewGuid(),
                Title = command.Title
            });
        }
    }
}