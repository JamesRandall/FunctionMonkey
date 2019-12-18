# Timers

First begin by creating an empty Azure Functions v2 project and then install the core nuget packages for Function Monkey:

    Install-Package FunctionMonkey
    Install-Package FunctionMonkey.Compiler

Now create a folder in the solution called commands and create a class called SendEmailCommand:

    public class CleanupCommand : ICommand
    {
        
    }

Next create a folder in the solution called Handlers and create a class called SendEmailCommandHandler:

    internal class CleanupCommandHandler : ICommandHandler<CleanupCommand>
    {
        public Task ExecuteAsync(CleanupCommand command)
        {
            // We won't really do anything here in this example
            return Task.CompletedTask;
        }
    }

And now we'll create our function app configuration in the root of the project that registers the command handler and registers the command with a 5 minute timer:

    public class FunctionAppConfiguration : IFunctionAppConfiguration
    {
        private const string QueueName = "sendEmailQueue";
        private const string ServiceBusConnectionName = "serviceBusConnection";

        public void Build(IFunctionHostBuilder builder)
        {
            builder
                .Setup((serviceCollection, commandRegistry) =>
                    commandRegistry.Register<CleanupCommandHandler>()
                )
                .Functions(functions => functions
                    .Timer<CleanUpCommand>("0 */5 * * * *")
                    );
        }
    }

And thats it! If you run this project you should find that the _CleanUpCommandHandler_ runs every 5 minutes.

## Creating Commands With Payloads

In the above example the CleanupCommand was created using its default constructor - if you want to create commands with populated properties then you can use a timer command factory.

First lets expand our command class to include a property:

    public class CleanupCommand : ICommand
    {
        public bool IsItUrgent { get; set; }
    }

And now we need to create a factory class, to do this we must implement the _ITimerCommandFactory<TCommand>_ interface:

    internal class CleanupCommandTimerCommandFactory : ITimerCommandFactory<CleanupCommand>
    {
        public CleanupCommand Create(string cronExpression)
        {
            return new CleanupCommand { IsItUrgent = true };
        }
    }

And finally update our function app configuration to instruct the function to use the factory:

    public class FunctionAppConfiguration : IFunctionAppConfiguration
    {
        private const string QueueName = "sendEmailQueue";
        private const string ServiceBusConnectionName = "serviceBusConnection";

        public void Build(IFunctionHostBuilder builder)
        {
            builder
                .Setup((serviceCollection, commandRegistry) =>
                    commandRegistry.Register<CleanupCommandHandler>()
                )
                .Functions(functions => functions
                    .Timer<CleanUpCommand, CleanupCommandTimerCommandFactory>("0 */5 * * * *")
                )
            );
        }
    }

And if you run this you should find your command handler is called with the IsItUrgent flag set to true. Timer command factories are created by the IoC container and so can have additional interfaces injected into them to source any relavant information.
