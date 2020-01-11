using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatRSample.Models;
using MediatRSample.Services;

namespace ToDo.Application.Services
{
    internal class Repository : IRepository
    {
        private readonly List<ToDoItem> _items = new List<ToDoItem>();
        
        public Task<ToDoItem> Create(string title)
        {
            ToDoItem newItem = new ToDoItem()
            {
                Title = title,
                Id = Guid.NewGuid(),
                IsComplete = false
            };
            _items.Add(newItem);
            return Task.FromResult(newItem);
        }

        public Task MarkComplete(Guid id)
        {
            ToDoItem item = _items.Single(x => x.Id == id);
            item.IsComplete = true;
            return Task.CompletedTask;
        }

        public Task<IReadOnlyCollection<ToDoItem>> GetAll()
        {
            IReadOnlyCollection<ToDoItem> items = _items;
            return Task.FromResult(items);
        }
    }
}