# Azure Storage Queue Quickstart

This quickstart walks through how to configure the commanding system to dispatch commands to queues and execute commands from queues.

A [sample is available](https://github.com/JamesRandall/AzureFromTheTrenches.Commanding/tree/master/Samples/AzureStorageQueueCommanding) that illustrates this however I'll also cover the important points below. I'm going to assume a couple of things in the below:

## Dispatching Commands

To get started you'll need to install a NuGet package:

    Install-Package AzureFromTheTrenches.Commanding.AzureStorage

And then register the Azure Storage Queue commanding system with your IoC container:

    ICommandRegistry registry = resolver.AddCommanding();
    resolver.AddQueues().AddAzureStorageCommanding();

Given a CloudQueue we can configure a command to be dispatched to a queue as shown below:

    CloudQueue myQueue = ...;
    registry.Register<AddCommand>(myQueue.CreateCommandDispatcher());

If you only have a connection string then you can accomplish the above without creating the CloudQueue with another helper method:

    string connectionString = ...;
    string queueName = ...;
    registry.Register<AddCommand>(CloudQueueDispatcherFactory.Create(connectionString, queueName));

You'll note in the above I don't specify a result type as queue dispatch is currently one way only with the processing happening asynchronously when the messages are pulled. Dispatching a command is the same as ever - where and how the command is executed is completely transparent to the dispatcher:

    await commandDispatcher.DispatchAsync(new AddCommand { FirstNumber = 5, SecondNumber = 6});

## Executing Queued Commands

Again, in addition to the core commanding package, we need to install the Azure Storage extensions in the project processing the queues (in Azure a WebJob for example):

    Install-Package AzureFromTheTrenches.Commanding.AzureStorage

And then we need to register the commanding system with our IoC container along with the command and its handler:

    ICommandRegistry registry = resolver.AddCommanding();
    resolver.AddQueues().AddAzureStorageCommanding();
    registry.Register<AddCommand, AddCommandHandler>();

This time we do register a handler along with our command.

Finally to have the command executed from a storage queue we need to obtain an instance of a processor factory and create a command processor that is pointed to our queue:

    IAzureStorageCommandQueueProcessorFactory processorFactory = resolver.Resolve<IAzureStorageCommandQueueProcessorFactory>();
    Task longRunningTask = processorFactory.Start<AddCommand, AddCommand>(queue, cancellationTokenSource.Token);

You need to manage the task execution and make sure your Web Job doesn't quit but that's a pretty standard pattern for a Web Job that I won't cover here though it can be seen in the sample.