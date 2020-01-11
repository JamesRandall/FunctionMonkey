using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatRSample.Models;

namespace MediatRSample.Services
{
    internal interface IRepository
    {
        Task<ToDoItem> Create(string title);

        Task MarkComplete(Guid id);

        Task<IReadOnlyCollection<ToDoItem>> GetAll();
    }
}