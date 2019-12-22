# Event Hubs

## Getting Started

First begin by creating an empty Azure Functions v2 project and then install the core nuget packages for Function Monkey:

    Install-Package FunctionMonkey
    Install-Package FunctionMonkey.Compiler

You will also need to add the Service Bus trigger bindings:

    Install-Package Microsoft.Azure.WebJobs.Extensions.EventHubs

Now create a folder in the solution called commands and create a class called Analytic:

    public class Analytic : ICommand
    {
        public string Fqn { get; set; }

        public int Value { get; set; }
    }

Next create a folder in the solution called Handlers and create a class called AnalyticHandler:

    internal class AnalyticHandler : ICommandHandler<Analytic>
    {
        public Task ExecuteAsync(Analytic analytic)
        {
            // We won't really send an email from this sample
            return Task.CompletedTask;
        }
    }

And now we'll create our function app configuration in the root of the project that registers the command handler and registers the command with a service bus queue:

    public class FunctionAppConfiguration : IFunctionAppConfiguration
    {
        private const string EventHubName = "analyticHub";
        
        public void Build(IFunctionHostBuilder builder)
        {
            builder
                .Setup((serviceCollection, commandRegistry) =>
                    commandRegistry.Register<Analytic>()
                )
                .Functions(functions => functions
                    .EventHub(eventHub => eventHub
                        .EventHubFunction<Analytic>(EventHubName)
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
        "eventHubConnectionString": "<connection string>" 
      }
    }

And that's it! If you run this project having created a queue called sendEmailQueue and use a real connection string you'll find that the _AnalyticHandler__ is invoked for each item you place on the event hub.

Note that it is possible to omit the connection setting name - see [default connection settings](/crosscutting/connectionStrings.md) for more details.

## Validation

If you want to validate the queue messages as you recieve them then validation is supported and is [documented here](/crosscutting/validation.html).
