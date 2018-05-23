# Error Handling

By default any errors in command handlers will bubble back up to the site of dispatch but be wrapped inside a CommandExecutionException that provides additional context so if we have a handler like so:

    public class MyCommandHandler : ICommandHandler<MyCommand>
    {
        public Task ExecuteAsync(MyCommand command)
        {
            throw new NotSupportedException();
        }
    }

Then we can trap our NotSupportedException at the site of dispatch as follows:

    public async Task DispatchMyCommand(ICommandDispatcher dispatcher)
    {
        try
        {
            await dispatcher.DispatchAsync(new MyCommand());
        }
        catch(CommandExecutionException ex)
        {
            Console.WriteLine($"Inner Exception: {ex.InnerException.GetType().Name}") // will write NotSupportedException
            Console.WriteLine("Success we caught it");
        }
    }

It is possible to override this standard behaviour (for example to do logging or take a conditional approach to exception rethrow) by implementing an instance of ICommandExecutionExceptionHandler such as that shown below:

    public class MyCustomExceptionHandler : ICommandExecutionExceptionHandler
    {
        public MyCustomExceptionHandler(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<bool> HandleException<TResult>(Exception ex, object handler, int handlerExecutionIndex, ICommand<TResult> command, ICommandDispatchContext dispatchContext)
        {
            if (ex is IgnoreException)
            {
                return true;
            }
            _logger.Error("Unexpected error occurred during command execution", ex);
            throw ex; // In our example this would cause the NotSupportedException to reach the dispatch point
        }
    }

Each exception raised during the execution of a handler will be passed to this method and it can rethrow the exception or take some other form of action. If an exception is not rethrown then the method should return true if the command system should continue to execute subsequent command handlers or return false to halt the execution pipeline (without throwing an exception).