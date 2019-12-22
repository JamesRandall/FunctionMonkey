using System.Collections.Generic;
using AzureFromTheTrenches.Commanding.Abstractions;
using FmAspNetCore.Model;

namespace FmAspNetCore.Commands
{
    public class GetAllTodoItemsQuery : ICommand<IReadOnlyCollection<TodoItem>>
    {
        
    }
}