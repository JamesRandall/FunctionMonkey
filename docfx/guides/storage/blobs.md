# Blobs

Function Monkey supports two approaches for responding to blob storage events:

1. Deserializing directly into a command from JSON - useful for small blobs who's format is known in advance
2. Assigning a stream to a command that can be picked up in the handler - useful for blobs who's format is not known or that are very large and require careful processing

## Deserializing Directly Into a Command

First begin by creating an empty Azure Functions v2 project and then install the core nuget packages for Function Monkey:

    Install-Package FunctionMonkey
    Install-Package FunctionMonkey.Compiler

You will also need the extension package for the storage triggers (at the time of writing this is 3.0.0-beta8):

    Install-Package Microsoft.Azure.WebJobs.Extensions.Storage

Now create a folder in the solution called commands and create a class called HelloWorldCommand:

    public class HelloWorldCommand : ICommand
    {
        public string UserName { get; set; }
    }

Next create a folder in the solution called Handlers and create a class called HelloWorldCommandHandler:

    internal class HelloWorldCommandHandler : ICommandHandler<HelloWorldCommand>
    {
        public Task ExecuteAsync(HelloWorldCommand command)
        {
            Debug.WriteLine($"Hello {command.UserName}");
            return Task.CompletedTask;
        }
    }

And now we'll create our function app configuration in the root of the project that registers the command handler and registers the command with a storage account and blob container:

    public class FunctionAppConfiguration : IFunctionAppConfiguration
    {
        private const string ContainerName = "myblobcontainer";
        private const string StorageConnectionName = "storageConnection";

        public void Build(IFunctionHostBuilder builder)
        {
            builder
                .Setup((serviceCollection, commandRegistry) =>
                    commandRegistry.Register<HelloWorldCommandHandler>()
                )
                .Functions(functions => functions
                    .Storage(StorageConnectionName, storage => storage
                        .BlobFunction<HelloWorldCommand>($"{ContainerName}/{name}")
                    )
                );
        }
    }

Finally we need to create an entry in local.settings.json for the Azure Storage connection string:

    {
      "IsEncrypted": false,
      "Values": {
        "AzureWebJobsStorage": "UseDevelopmentStorage=true",
        "AzureWebJobsDashboard": "UseDevelopmentStorage=true",
        "storageConnection": "<connection string>" 
      }
    }

And that's it! If you run this sample you should find that adding a blob to the container triggers the function. Note that Azure Functions blob triggers do not work with the storage emulator, you need to use a real storage account.

Note that it is possible to omit the connection setting name - see [default connection settings](/crosscutting/connectionStrings.md) for more details.

## Using Streams

First begin by creating an empty Azure Functions v2 project and then install the core nuget packages for Function Monkey:

    Install-Package FunctionMonkey
    Install-Package FunctionMonkey.Compiler

Now create a folder in the solution called commands and create a class called HelloWorldCommand - in addition to implementing the _ICommand_ interface we also must implement the _IStreamCommand_:

    public class HelloWorldCommand : ICommand, IStreamCommand
    {
        public Stream Stream { get; set; }

        public string Name { get; set; }
    }

Next create a folder in the solution called Handlers and create a class called HelloWorldCommandHandler:

    internal class HelloWorldCommandHandler : ICommandHandler<HelloWorldCommand, Message>
    {
        public Task<Message> ExecuteAsync(HelloWorldCommand command, Message previousResult)
        {
            using (StreamReader reader = new StreamReader(command.Stream))
            {
                string json = reader.ReadToEnd();
                return Task.FromResult(new Message
                {
                    Text = $"JSON payload\n{json}"
                });
            }                
        }
    }

Our handler will be provided an open stream to the blob in the _Stream_ property and the name of the blob in the _Name_ property of the command.
    
And now we'll create our function app configuration in the root of the project that registers the command handler and registers the command with a storage account and blob container:

    public class FunctionAppConfiguration : IFunctionAppConfiguration
    {
        private const string ContainerName = "myblobcontainer";
        private const string StorageConnectionName = "storageConnection";

        public void Build(IFunctionHostBuilder builder)
        {
            builder
                .Setup((serviceCollection, commandRegistry) =>
                    commandRegistry.Register<HelloWorldCommandHandler>()
                )
                .Functions(functions => functions
                    .Storage(StorageConnectionName, storage => storage
                        .BlobFunction<HelloWorldCommand>($"{ContainerName}/{name}")
                    )
                );
        }
    }

Finally we need to create an entry in local.settings.json for the Azure Storage connection string:

    {
      "IsEncrypted": false,
      "Values": {
        "AzureWebJobsStorage": "UseDevelopmentStorage=true",
        "AzureWebJobsDashboard": "UseDevelopmentStorage=true",
        "storageConnection": "<connection string>" 
      }
    }

And that's it! If you run this sample you should find that adding a blob to the container triggers the function and you're given an open stream. Note that Azure Functions blob triggers do not work with the storage emulator, you need to use a real storage account.