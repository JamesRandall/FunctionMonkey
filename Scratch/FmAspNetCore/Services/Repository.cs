using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FmAspNetCore.Model;

namespace FmAspNetCore.Services
{
    internal class Repository : IRepository
    {
        private readonly List<TodoItem> _collection = new List<TodoItem>();
        
        public Task<TodoItem> Create(string title)
        {
            TodoItem item = new TodoItem()
            {
                Id = Guid.NewGuid(),
                IsComplete = false,
                Title = title
            };
            _collection.Add(item);
            return Task.FromResult(item);
        }

        public Task<IReadOnlyCollection<TodoItem>> GetAll()
        {
            return Task.FromResult((IReadOnlyCollection<TodoItem>)_collection);
        }

        public Task MarkComplete(Guid id)
        {
            _collection.Single(x => x.Id == id).IsComplete = true;
            return Task.CompletedTask;
        }
    }
}