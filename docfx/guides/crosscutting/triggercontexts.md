# Trigger Contexts

In addition to the input payload Azure Functions also have access to other contextual information depending on their trigger type - for example Azure Storage Queue triggers have access to the expiration date of the message, the dequeue count, and more.

Function Monkey makes these available to command handlers via the injectable _IContextProvider_ interface.

The ExecutionContext property is available for all trigger types and exposes the WebJobs.ExecutionContext information: for example the (very useful!) invocation ID.

Depending on the trigger type the following other context properties are also available:

|Context|Availability|
|-------|------------|
|ServiceBusContext|Available for service bus queue and topic / subscription functions|
|StorageQueueContext|Available for Azure Storage queue functions|
|BlobContext|Available for Azure Storage blob trigger functions|
|EventHubContext|Available for Azure Event Hub trigger functions|
|HttpContext|Available for HTTP triggers and provides access to the headers and request URL|

Note that one of the advantages of the mediator pattern that underpins Function Monkey is the clean separation of concerns including application logic from host / protocol but by making use of these contexts that decouplign is weakened. An alternative approach can be to handle these things in a decorated mediator.

For further details see the [API Guide](https://functionmonkey.azurefromthetrenches.com/api/FunctionMonkey.Abstractions.Contexts.html).

