# Cosmos Change Feed

## Getting Started

First begin by creating an empty Azure Functions v2 project and then install the core nuget packages for Function Monkey:

    Install-Package FunctionMonkey
    Install-Package FunctionMonkey.Compiler

You will also need to add the Cosmos trigger bindings:

    Install-Package Microsoft.Azure.WebJobs.Extensions.CosmosDB

Now create a folder in the solution called commands and create a class called InvoiceUpdateCommand that matches the document schema in Cosmos:

    public class Address
    {
        public string Line1 { get; set; }

        public string Line2 { get; set; }

        public string Line3 { get; set; }

        public string Postcode { get; set; }

        public string Counter { get; set;}
    }

    public class InvoiceUpdatedCommand : ICommand
    {
        public string Id { get; set; }

        public Address Address { get; set; }

        public double Value { get; set; }
    }

Next create a folder in the solution called Handlers and create a class called InvoiceUpdatedCommandHandler:

    internal class InvoiceUpdatedCommandHandler : ICommandHandler<InvoiceUpdatedCommand>
    {
        public Task ExecuteAsync(InvoiceUpdatedCommand command)
        {
            // We won't really do anything in this sample
            return Task.CompletedTask;
        }
    }

And now we'll create our function app configuration in the root of the project that registers the command handler and registers the command with a Cosmos instance:

    public class FunctionAppConfiguration : IFunctionAppConfiguration
    {
        private const string CosmosDatabaseName = "Financials";
        private const string CosmosCollectionName = "Invoices";
        private const string CosmosConnectionSettingName = "cosmosDbConnection";

        public void Build(IFunctionHostBuilder builder)
        {
            builder
                .Setup((serviceCollection, commandRegistry) =>
                    commandRegistry.Register<InvoiceUpdatedCommandHandler>()
                )
                .Functions(functions => functions
                    .CosmosDb(CosmosConnectionSettingName, cosmos => cosmos
                        .ChangeFeedFunction<InvoiceUpdatedCommand>(CosmosCollectionName, CosmosDatabaseName, convertToPascalCase:true)
                    )
                );
        }
    }

The above Cosmos function will monitor the change feed for the Invoices collection inside the Financials database and use the default trigger settings and so expect to find a collection called _leases_ also inside the Financials database.

By setting convertToPascalCase to true Function Monkey will handle the camel / Pascal case issues that exist within the Cosmos change feed processor and downstream in Azure Functions. Note that due to limitations in the Cosmos NuGet package this involves a serialization and deserialization step and so does carry overhead. The alternative approach is to use camel case in your command or to mark each property in the command (and referenced model) with the JsonProperty attribute and specify a camel case property name. Messy, I realise, but the limitation is in how the Cosmos team have exposed serialization (or rather how they haven't) on the Change Feed Processor - hopefully they will address this.

Finally we need to create an entry in local.settings.json for the Cosmos connection string:

    {
      "IsEncrypted": false,
      "Values": {
        "AzureWebJobsStorage": "UseDevelopmentStorage=true",
        "AzureWebJobsDashboard": "UseDevelopmentStorage=true",
        "cosmosDbConnection": "<connection string>" 
      }
    }

And that's it! If you run this project against an appropriate Cosmos database and make changes to the documents you will find they are sent to the appropriate Cosmos handler.

Note that it is possible to omit the connection setting name - see [default connection settings](/crosscutting/connectionStrings.md) for more details.

## Using Documents Directly

Rather than constructing commands from the Cosmos Documents, Function Monkey also supports dealing with the Document class directly via the _ICosmosDbDocumentCommand_ and the _ICosmosDbDocumentBatchCommand_ interfaces that can be found in the _FunctionMonkey.Commanding.Cosmos.Abstractions_ NuGet package.

To deal with a single document simply implement the _ICosmosDbDocumentCommand_:

    public class InvoiceUpdatedCommand : ICosmosDbDocumentCommand
    {
        public Document Document { get; set; }
    }

Or to deal with them in batch (as fed to the Azure Function):

    public class InvoiceUpdatedCommand : _ICosmosDbDocumentBatchCommand_
    {
        public Document Documents { get; set; }
    } 

## Validation

If you want to validate the queue messages as you recieve them then validation is supported and is [documented here](/crosscutting/validation.html).

Validation only takes place when *not* using the _ICosmosDbDocumentCommand_ or the _ICosmosDbDocumentBatchCommand_ interface.