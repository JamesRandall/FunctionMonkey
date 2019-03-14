using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.FluentValidation;
using FunctionMonkey.Tests.Integration.Commands;

namespace FunctionMonkey.Tests.Integration
{
    public class FunctionAppConfiguration : IFunctionAppConfiguration
    {
        // In the HTTP routes we need to cover the following variations of response (it gets complicated due to permutations with the response
        // handler and validation)
        //  1.  No response handler and a command with a result and no validation
        //  2.  No response handler and a command with a result and validation that passes
        //  3.  No response handler and a command with a result and validation that fails
        //  4.  No response handler and a command with no result and no validation
        //  5.  No response handler and a command with no result and validation that passes
        //  6.  No response handler and a command with no result and validation that fails
        //  7.  A response handler for a command with a result and no validation
        //  8.  A response handler for a command with no result and no validation
        //  9.  A response handler for a command with a result and a validator that passes - this should hit the CreateResponse method
        //  10. A response handler for a command with a result and a validator that fails - this should hit the CreateValidationFailureResponse method
        //  11. A response handler for a command with no result and a validator that passes - this should hit the CreateResponse method
        //  12. A response handler for a command with a result and a validator that fails - this should hit the CreateValidationFailureResponse method
        public void Build(IFunctionHostBuilder builder)
        {
            builder
                .Setup((serviceCollection, commandRegistry) =>
                {
                    serviceCollection
                        .AddValidatorsFromAssemblyContaining<FunctionAppConfiguration>()                        
                    ;
                    commandRegistry.Discover<FunctionAppConfiguration>();
                })
                .AddFluentValidation()
                //.OutputAuthoredSource(@"c:\wip\outputsource")
                .Functions(functions => functions
                    .HttpRoute("simpleHttp", route => route
                        .HttpFunction<SimpleHttpRouteCommand>()
                        // These are the functions for testing the HTTP route cases outlined above
                        .HttpFunction<CommandWithNoResultAndNoValidation>()
                        .HttpFunction<CommandWithNoResultAndValidatorThatFails>()
                        .HttpFunction<CommandWithNoResultAndValidatorThatPasses>()
                        .HttpFunction<CommandWithResultAndNoValidation>()
                        .HttpFunction<CommandWithResultAndValidatorThatFails>()
                        .HttpFunction<CommandWithResultAndValidatorThatPasses>()
                        .HttpFunction<ResponseHandlerCommandWithNoResultAndNoValidation>()
                            .Options(options => options.ResponseHandler<CustomResponseHandler>())
                        .HttpFunction<ResponseHandlerCommandWithNoResultAndValidatorThatFails>()
                            .Options(options => options.ResponseHandler<CustomResponseHandler>())
                        .HttpFunction<ResponseHandlerCommandWithNoResultAndValidatorThatPasses>()
                            .Options(options => options.ResponseHandler<CustomResponseHandler>())
                        .HttpFunction<ResponseHandlerCommandWithResultAndNoValidation>()
                            .Options(options => options.ResponseHandler<CustomResponseHandler>())
                        .HttpFunction<ResponseHandlerCommandWithResultAndValidatorThatFails>()
                            .Options(options => options.ResponseHandler<CustomResponseHandler>())
                        .HttpFunction<ResponseHandlerCommandWithResultAndValidatorThatPasses>()
                            .Options(options => options.ResponseHandler<CustomResponseHandler>())
                    )
                    .Storage("storageAccount", storage => storage
                        .QueueFunction<StorageQueueCommand>("myqueue")
                        .BlobFunction<BlobCommand>("blobcommandcontainer/{name}") // TODO: We need to have the compiler insert parameters on the function for everything surrounded in {} - e.g. {name} needs a string parameter of name
                        .BlobFunction<StreamBlobCommand>("streamblobcommandcontainer/{name}")
                    )
                    .CosmosDb("cosmosConnection", cosmos => cosmos
                        .ChangeFeedFunction<SimpleCosmosChangeFeedCommand>("cosmosCollection", "cosmosDatabase")
                    )
                    .ServiceBus("serviceBuConnection", serviceBus => serviceBus
                        .QueueFunction<SimpleServiceBusQueueCommand>("myqueue")
                        .SubscriptionFunction<SimpleServiceBusTopicCommand>("mytopic", "mysubscription")
                    )
                );
        }
    }
}
