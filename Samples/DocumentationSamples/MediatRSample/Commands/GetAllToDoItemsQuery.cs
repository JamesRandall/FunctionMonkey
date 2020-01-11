using System.Collections.Generic;
using MediatR;
using MediatRSample.Models;

namespace ToDo.Application.Commands
{
    public class GetAllToDoItemsQuery : IRequest<IReadOnlyCollection<ToDoItem>>
    {
        
    }
}