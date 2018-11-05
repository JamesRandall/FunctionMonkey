# Error Handling

Unless you use the _ICosmosDbDocumentBatchCommand_ interface on your commands then Function Monkey will process batches of documents in the order they are recieved - one invocation of your function may contain multiple documents.

If an error occurs, either in the Function Monkey wrapper or due to an exception thrown from your command handler, then the default behaviour is to log an error along with the document ID and continue to process documents.

You can extend and modify this behaviour by implementing and registering an instance of the _ICosmosDbErrorHandler_ interface. This will allow you to take additional steps (for example initiate some form of retry), log information yourself, and stop the processing of the current batch.

The example implementation below logs a warning and allows processing of the current batch to continue:

    public class ExampleCosmosErrorHandler : ICosmosDbErrorHandler
    {
        private readonly ILogger<ExampleCosmosErrorHandler> _logger;

        public ExampleCosmosErrorHandler(ILogger<ExampleCosmosErrorHandler> logger)
        {
            _logger = logger;
        }

        public Task<bool> HandleError(Exception ex, Document document)
        {
            _logger.LogWarning(ex, "Ooops");
            return Task.FromResult(true); // return false to stop the processing of the current batch
        }
    }

This can be registered on a per function basis as shown in the example configuration block below:

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
                        .ChangeFeedFunction<InvoiceUpdatedCommand, ExampleCosmosErrorHandler>(CosmosCollectionName, CosmosDatabaseName, convertToPascalCase:true)
                    )
                );
        }
    }
