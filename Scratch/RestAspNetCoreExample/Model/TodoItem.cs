using System;

namespace RestAspNetCoreExample.Model
{
    public class TodoItem
    {
        public Guid Id { get; set; }
        
        public string Title { get; set; }
    }
}