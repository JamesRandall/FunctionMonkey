# Service Bus Queues

## Getting Started

First begin by creating an empty Azure Functions v2 project and then install the core nuget packages for Function Monkey:

    Install-Package FunctionMonkey
    Install-Package FunctionMonkey.Compiler

You will also need to add the Service Bus trigger bindings:

    Install-Package Microsoft.Azure.WebJobs.ServiceBus

Now create a folder in the solution called commands and create a class called SendEmailCommand:

    public class SendEmailCommand : ICommand
    {
        public string To { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }
    }

Next create a folder in the solution called Handlers and create a class called SendEmailCommandHandler:

    internal class SendEmailCommandHandler : ICommandHandler<SendEmailCommand>
    {
        public Task ExecuteAsync(SendEmailCommand command)
        {
            // We won't really send an email from this sample
            return Task.CompletedTask;
        }
    }

And now we'll create our function app configuration in the root of the project that registers the command handler and registers the command with a service bus queue:

    public class FunctionAppConfiguration : IFunctionAppConfiguration
    {
        private const string QueueName = "sendEmailQueue";
        private const string ServiceBusConnectionName = "serviceBusConnection";

        public void Build(IFunctionHostBuilder builder)
        {
            builder
                .Setup((serviceCollection, commandRegistry) =>
                    commandRegistry.Register<SendEmailCommandHandler>()
                )
                .Functions(functions => functions
                    .ServiceBus(ServiceBusConnectionName, serviceBus => serviceBus
                        .QueueFunction<SendEmailCommand>(QueueName)
                    )
                );
        }
    }

Finally we need to create an entry in local.settings.json for the Service Bus connection string:

    {
      "IsEncrypted": false,
      "Values": {
        "AzureWebJobsStorage": "UseDevelopmentStorage=true",
        "AzureWebJobsDashboard": "UseDevelopmentStorage=true",
        "serviceBusConnection": "<connection string>" 
      }
    }

And that's it! If you run this project having created a queue called sendEmailQueue and use a real connection string you'll find that the _SendEmailCommandHandler_ is invoked for each item you place on the queue.

Note that it is possible to omit the connection setting name - see [default connection settings](/crosscutting/connectionStrings.md) for more details.

## Validation

If you want to validate the queue messages as you recieve them then validation is supported and is [documented here](/crosscutting/validation.html).

## Sample Code

The source code for the above can be found over in [GitHub](https://github.com/JamesRandall/FunctionMonkey/tree/master/Samples/DocumentationSamples/ServiceBusSample).