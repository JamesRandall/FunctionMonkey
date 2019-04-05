# Service Bus Topics

## Getting Started

First begin by creating an empty Azure Functions v2 project and then install the core nuget packages for Function Monkey:

    Install-Package FunctionMonkey
    Install-Package FunctionMonkey.Compiler

You will also need to add the Service Bus trigger bindings:

    Install-Package Microsoft.Azure.WebJobs.ServiceBus

In our hyptotheitcal solution we're going to listen to new user registration messages on a topic subscription, begin by Now creating a folder in the solution called commands and create a class called NewUserRegistrationCommand:

    public class NewUserRegistrationCommand : ICommand
    {
        public Guid UserId { get; set; }

        public string Username { get; set; }

        public string EmailAddress { get; set; }
    }

Next create a folder in the solution called Handlers and create a class called NewUserRegistrationCommandHandler:

    internal class NewUserRegistrationCommandHandler : ICommandHandler<NewUserRegistrationCommand>
    {
        public Task ExecuteAsync(NewUserRegistrationCommand command)
        {
            // We're not really going to do anything here
            return Task.CompletedTask;
        }
    }

And now we'll create our function app configuration in the root of the project that registers the command handler and registers the command with a service bus topic and subscription:

    public class FunctionAppConfiguration : IFunctionAppConfiguration
    {
        private const string TopicName = "newuserregistration";
        private const string SubscriptionName = "telemetrysubscription";
        private const string ServiceBusConnectionName = "serviceBusConnection";

        public void Build(IFunctionHostBuilder builder)
        {
            builder
                .Setup((serviceCollection, commandRegistry) =>
                    commandRegistry
                        .Register<NewUserRegistrationCommandHandler>()
                )
                .Functions(functions => functions
                    .ServiceBus(ServiceBusConnectionName, serviceBus => serviceBus
                        .SubscriptionFunction<NewUserRegistrationCommand>(TopicName, SubscriptionName)
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

And that's it! If you run this project having created a topic and subscription with the appropriate names and use a real connection string you'll find that the _SendEmailCommandHandler_ is invoked for each item you place on the queue.

Note that it is possible to omit the connection setting name - see [default connection settings](/crosscutting/connectionStrings.md) for more details.

## Validation

If you want to validate the queue messages as you recieve them then validation is supported and is [documented here](/crosscutting/validation.html).

## Sample Code

The source code for the above can be found over in [GitHub](https://github.com/JamesRandall/FunctionMonkey/tree/master/Samples/DocumentationSamples/ServiceBusSample).