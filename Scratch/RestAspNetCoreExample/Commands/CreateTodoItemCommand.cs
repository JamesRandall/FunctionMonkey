using AzureFromTheTrenches.Commanding.Abstractions;
using RestAspNetCoreExample.Model;

namespace RestAspNetCoreExample.Commands
{
    public class CreateTodoItemCommand : ICommand<TodoItem>
    {
        public string Title { get; set; }
    }
}