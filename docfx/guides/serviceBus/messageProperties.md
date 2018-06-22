# Service Bus Message Properties

Sometimes it can be useful to access the properties that accompany a Service Bus message, for example the dequeue count. To do that inject the _IContextProvider_ interface into your handler and use the _ServiceBusContext_ property to access the properties:

    internal class SendEmailCommandHandler : ICommandHandler<SendEmailCommand>
    {
        private readonly IContextProvider _contextProvider;

        public SendEmailCommandHandler(IContextProvider contextProvider)
        {
            _contextProvider = contextProvider;
        }

        public Task ExecuteAsync(SendEmailCommand command)
        {
            if (_contextProvider.ServiceBusContext.DeliveryCount >= 5)
            {
                // do something else
            }
            // We won't really send an email from this sample
            return Task.CompletedTask;
        }
    }

Properties available include:

Property|Description
--------|-----------
DeliveryCount|The number of times the message has been delivered
EnqueuedTimeUTc|The date and time the message was enqueued
MessageId|The ID of the message

Care should be taken when doing this as it removes one of the benefits of the mediator pattern - the abstraction between caller and handler and essentially tightly couples the handler to the service bus. An alternative approach that maintains the decoupling and separation of concerns would be to implement a decorated _ICommandDispatcher_ (see the AzureFromTheTrenches.Commanding documentation).
