# Message Sessions

Function Monkey supports (FIFO message sessions)[https://docs.microsoft.com/en-us/azure/service-bus-messaging/message-sessions] for both message processing and output triggers.

To enable on a Service Bus queue or subscription function set the isSessionEnabled parameter to true. For example:

    .ServiceBus(serviceBus => serviceBus
        .QueueFunction<ServiceBusSessionIdQueueCommand>("myqueue", true)
    )

To specify the message ID in a service bus output binding you need to supply the session ID in the command response and tell Function Monkey which property contains the ID. For example:

    .HttpRoute("outputBindings", route => route
        .HttpFunction<MyCommand>("/toServiceBusQueueWithResultSessionId")
        .OutputTo.ServiceBusQueue<MyCommandResult>("myqueue", result => result.SessionId)
    )