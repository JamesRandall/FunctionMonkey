using System;

namespace FmAspNetCore.Model
{
    public class TodoItem
    {
        public Guid Id { get; set; }
        
        public string Title { get; set; }
        
        public bool IsComplete { get; set; }
    }
}