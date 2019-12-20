using AzureFromTheTrenches.Commanding.Abstractions;
using FmAspNetCore.Model;

namespace FmAspNetCore.Commands
{
    public class CreateTodoItemCommand : ICommand<TodoItem>
    {
        public string Title { get; set; }
    }
}