using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FmAspNetCore.Model;

namespace FmAspNetCore.Services
{
    internal interface IRepository
    {
        Task<TodoItem> Create(string title);
        Task<IReadOnlyCollection<TodoItem>> GetAll();
        Task MarkComplete(Guid id);
    }
}