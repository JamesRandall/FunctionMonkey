using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestAspNetCoreExample.Commands;
using RestAspNetCoreExample.Model;

namespace RestAspNetCoreExample
{
    [ApiController]
    public class TodoItemController : ControllerBase
    {
        private readonly ICommandDispatcher _dispatcher;

        public TodoItemController(ICommandDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }
        
        [Route("todo")]
        [HttpPost]
        public async Task<ActionResult<TodoItem>> Handler(CreateTodoItemCommand command)
        {
            
            TodoItem result = await _dispatcher.DispatchAsync(command);
            return result;
        }
    }
}