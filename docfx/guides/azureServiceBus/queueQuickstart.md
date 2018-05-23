# Azure Service Bus Quickstart

This quickstart walks through how to configure the commanding system to dispatch commands over Azure Service Bus Queues and Topics and then execute them from Queues and Subscriptins.

A simple sample is available here:

[https://github.com/JamesRandall/AzureFromTheTrenches.Commanding/tree/master/Samples/ServiceBusDispatchAndDequeue](https://github.com/JamesRandall/AzureFromTheTrenches.Commanding/tree/master/Samples/ServiceBusDispatchAndDequeue)

## Dispatching to a Queue or Topic

To get started you'll need to install a NuGet package:

    Install-Package AzureFromTheTrenches.Commanding.AzureServiceBus

And then register the Azure Storage Queue commanding system with your IoC container:

    ICommandRegistry registry = resolver.AddCommanding();
    resolver.AddAzureServiceBus();

Given a QueueClient we can configure a command to be dispatched to it as shown below:

    QueueClient client = new QueueClient(serviceBusConnectionString, "myqueue");
    commandRegistry.Register<AddCommand>(client.CreateCommandDispatcherFactory());

If you want to dispatch to a Topic instead the syntax is basically the same:

    TopicClient client = new TopicClient(serviceBusConnectionString, "mytopic");
    commandRegistry.Register<AddCommand>(client.CreateCommandDispatcherFactory());

Dispatching a command is the same as ever - where and how the command is executed is completely transparent to the dispatcher:

    await commandDispatcher.DispatchAsync(new AddCommand { FirstNumber = 5, SecondNumber = 6});

## Executing Queued Commands from Queues and Subscriptions

Like for dispatch you'll first need to install a NuGet package:

    Install-Package AzureFromTheTrenches.Commanding.AzureServiceBus

And then we need to register the commanding system with our IoC container along with the command and its handler:

    ICommandRegistry registry = resolver.AddCommanding();
    resolver.AddQueues().AddAzureServiceBus();
    registry.Register<AddCommand, AddCommandHandler>();

And finally we create a Service Bus command queue processor:

    QueueClient client = new QueueClient(serviceBusConnectionString, "myqueue");
    IServiceBusCommandQueueProcessor commandQueueProcessor = factory.Create<AddCommand>(client);

If you want to read from a Subscription rather than a queue then the syntax is basically the same:

    SubscriptionClient client = new SubscriptionClient(serviceBusConnectionString, "mytopic", "mysubscription");
    IServiceBusCommandQueueProcessor commandQueueProcessor = factory.Create<AddCommand>(client);

Note that you need to keep a reference to the commandQueueProcessor if you don't want it to shut down.
