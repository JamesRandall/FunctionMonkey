using System;
using MediatR;

namespace ToDo.Application.Commands
{
    public class MarkToDoItemCompleteCommand : INotification
    {
        public Guid Id { get; set; }
    }
}