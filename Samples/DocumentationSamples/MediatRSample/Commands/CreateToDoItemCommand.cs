using MediatR;
using MediatRSample.Models;

namespace ToDo.Application.Commands
{
    public class CreateToDoItemCommand : IRequest<ToDoItem>
    {
        public string Title { get; set; }
    }
}