# SignalR

The following assumes a basic familiarity with serverless SignalR as [can be found in this guide here](https://docs.microsoft.com/en-us/azure/azure-signalr/signalr-concept-azure-functions).

The examples below use [default connection strings](/crosscutting/connectionStrings.md).

Before beginning you will need to install the Function Monkey SignalR package to your Functions project:

    Install-Package FunctionMonkey.SignalR

## Providing a client with an access token and endpoint

For a client to be able to listen for SignalR events it needs to connect to the SignalR hub and to do this it requires the URL of the hub and an access token. This is generally provided over an HTTP endpoint and typically behind some form of authentication / authorization mechanism.

Function Monkey provides two ways for you to implement such an endpoint.

### Using a binding expression

Function Monkey allows for the access token and endpoint to be returned using the same approach as Azure Functions. For example here is the standard Azure Functions approach to returning the token:

    [FunctionName("negotiate")]
    public static SignalRConnectionInfo Negotiate(
        [HttpTrigger(AuthorizationLevel.Anonymous)]HttpRequest req, 
        [SignalRConnectionInfo
            (HubName = "chat", UserId = "{headers.x-ms-client-principal-id}")]
            SignalRConnectionInfo connectionInfo)
    {
        // connectionInfo contains an access key token with a name identifier claim set to the authenticated user
        return connectionInfo;
    }

This returns an access token for the hub called chat and with a user ID that is populated from the x-ms-client-principal-id header (this is a header set by the App Service easy auth system).

The Function Monkey equivelant of this looks like the below:

    public class FunctionAppConfiguration : IFunctionAppConfiguration
    {
        public void Build(IFunctionHostBuilder builder)
        {
            builder
                .Functions(functions => functions
                    .SignalR(signalR => signalR
                        .Negotiate("/negotiate", "chat", "{headers.x-ms-client-principal-id}")
                    )
                );
        }
    }

Note that the user ID binding expression is optional.

## Using a command

If, for example, the user ID is not available in a header but instead is in, for example, a database or a claim then Function Monkey also includes the ability for the access token to be returned by way of a command. In this case the _IFunctionAppConfiguration_ looks like the below:

    public class FunctionAppConfiguration : IFunctionAppConfiguration
    {
        public void Build(IFunctionHostBuilder builder)
        {
            builder
                .Functions(functions => functions
                    .SignalR(signalR => signalR
                        .Negotiate<SignalRNegotiateCommand>("/negotiate")
                    )
                );
        }
    }

The SignalRNegotiateCommand must return a _SignalRNegotiateResponse_ as shown below:

    public class SignalRNegotiateCommand : ICommand<SignalRNegotiateResponse>
    {
        
    }

An implementation for this in a command handler (using hard coded values) could look like the below:

    internal class NegotiateCommandHandler : ICommandHandler<SignalRNegotiateCommand, SignalRNegotiateResponse>
    {
        public Task<SignalRNegotiateResponse> ExecuteAsync(NegotiateCommand command, SignalRNegotiateResponse previousResult)
        {
            return Task.FromResult(new SignalRNegotiateResponse
            {
                HubName = "myhub",
                UserId = "1234"
            });
        }
    }

## Sending messages to clients

Sending messages to SignalR clients is accomplished through an output binding. The example below illustrates how to configure Function Monkey to send a message in response to processing an item from a Service Bus queue:

    public class FunctionAppConfiguration : IFunctionAppConfiguration
    {
        public void Build(IFunctionHostBuilder builder)
        {
            builder
                .Functions(functions => functions
                    .ServiceBus(serviceBus => serviceBus
                        .QueueFunction<ProcessOrderCommand>("orderQueue")
                        .OutputTo.SignalRMessage("myhub")  
                    )
                );
        }
    }

Commands that send SignalR messages must have a return type of _SignalRMessage_:

    public class ProcessOrderCommand : ICommand<SignalRMessage>
    {
        public string OrderNumber { get; set; }

        public string OrderedByUserId { get; set; }
    }

And a simple handler for this would take the following form:

    internal class ProcessOrderCommandHandler : ICommandHandler<ProcessOrderCommand, SignalRMessage>
    {
        public Task<SignalRMessage> ExecuteAsync(ProcessOrderCommand command, SignalRMessage previousResult)
        {
            return Task.FromResult(new SignalRMessage
            {
                Arguments = new object[] { command.OrderNumber },
                GroupName = null,
                Target = "orderProcessedNotification",
                UserId = command.OrderedByUserId
            });
        }
    }

The _GroupName_ property can be set to the name of any SignalR group (see below) and the _UserId_ property is optional. If it is not set then the message will be sent to all users.

## Adding and removing users from groups

Adding and removing users from groups takes a similar form to sending messages and is again accomplished through an output binding:

    public class FunctionAppConfiguration : IFunctionAppConfiguration
    {
        public void Build(IFunctionHostBuilder builder)
        {
            builder
                .Functions(functions => functions
                    .ServiceBus(serviceBus => serviceBus
                        .QueueFunction<AddUserToMarketingGroupCommand>("userManagementQueue")
                        .OutputTo.SignalRMessage("myhub")  
                    )
                );
        }
    }

Commands that manipulate SignalR groups must have a return type of _SignalRGroupAction_:

    public class AddUserToMarketingGroupCommand : ICommand<SignalRGroupAction>
    {
        public string UserId { get; set; }
    }

And a simple handler for this would take the following form:

    internal class AddUserToMarketingGroupCommandHandler : ICommandHandler<AddUserToMarketingGroupCommand, SignalRMessage>
    {
        public Task<SignalRGroupAction> ExecuteAsync(AddUserToMarketingGroupCommand command, SignalRGroupAction previousResult)
        {
            return Task.FromResult(new SignalRGroupAction
            {
                Action = GroupActionEnum.Add,
                GroupName = "marketingInfo",
                UserId = command.UserId
            });
        }
    }

## SignalRMessage and SignalRGroupAction

These two POCOs have exactly the same form as those that can be found in the Azure Functions WebJobs extension package but live inside the FunctionMonkey.Abstractions package and you must take care to ensure you use these classes.

The reason I've cloned the POCOs like this is because the Azure Functions variants are bundled inside the extension package that has a large number of dependencies that are not appropriate in the business / domain tier of an application.

Hopefully in time more teams within Microsoft will adopt the *.Abstractions form for packages.