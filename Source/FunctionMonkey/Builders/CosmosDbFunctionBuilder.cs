using System.Collections.Generic;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Abstractions.Builders.Model;
using FunctionMonkey.Abstractions.Extensions;
using FunctionMonkey.Commanding.Cosmos.Abstractions;
using FunctionMonkey.Extensions;
using FunctionMonkey.Model;

namespace FunctionMonkey.Builders
{
    internal class CosmosDbFunctionBuilder : ICosmosDbFunctionBuilder
    {
        private readonly ConnectionStringSettingNames _connectionStringSettingNames;
        private readonly string _connectionStringName;
        private readonly string _leaseConnectionStringName;
        private readonly List<AbstractFunctionDefinition> _functionDefinitions;

        public CosmosDbFunctionBuilder(
            ConnectionStringSettingNames connectionStringSettingNames,
            string connectionStringName,
            string leaseConnectionName,
            List<AbstractFunctionDefinition> functionDefinitions)
        {
            _connectionStringSettingNames = connectionStringSettingNames;
            _connectionStringName = connectionStringName;
            _functionDefinitions = functionDefinitions;
            _leaseConnectionStringName = leaseConnectionName;
        }

        public ICosmosDbFunctionOptionBuilder<TCommand> ChangeFeedFunction<TCommand>(
            string collectionName,
            string databaseName,
            string leaseCollectionName = "leases",
            string leaseDatabaseName = null,
            bool createLeaseCollectionIfNotExists = false,
            bool startFromBeginning = false,
            bool convertToPascalCase = true,
            string leaseCollectionPrefix = null,
            int? maxItemsPerInvocation = null,
            int? feedPollDelay = null,
            int? leaseAcquireInterval = null,
            int? leaseExpirationInterval = null,
            int? leaseRenewInterval = null,
            int? checkpointFrequency = null,
            int? leasesCollectionThroughput = null,
            bool trackRemainingWork = false,
            string remainingWorkCronExpression = "*/5 * * * * *"
            )
        {
            CosmosDbFunctionDefinition definition = new CosmosDbFunctionDefinition(typeof(TCommand))
            {
                ConnectionStringName = _connectionStringName,
                CollectionName = collectionName,
                DatabaseName = databaseName,
                LeaseConnectionStringName = _leaseConnectionStringName,
                LeaseCollectionName = leaseCollectionName,
                LeaseDatabaseName = leaseDatabaseName ?? databaseName,
                CreateLeaseCollectionIfNotExists = createLeaseCollectionIfNotExists,
                ConvertToPascalCase = convertToPascalCase,
                StartFromBeginning = startFromBeginning,
                LeaseCollectionPrefix = leaseCollectionPrefix,
                MaxItemsPerInvocation = maxItemsPerInvocation,
                FeedPollDelay = feedPollDelay,
                LeaseAcquireInterval = leaseAcquireInterval,
                LeaseExpirationInterval = leaseExpirationInterval,
                LeaseRenewInterval = leaseRenewInterval,
                CheckpointFrequency = checkpointFrequency,
                LeasesCollectionThroughput = leasesCollectionThroughput,
                TrackRemainingWork = trackRemainingWork,
                RemainingWorkCronExpression = remainingWorkCronExpression
            };
            _functionDefinitions.Add(definition);
            return new CosmosDbFunctionOptionBuilder<TCommand>(_connectionStringSettingNames, this, definition);
        }

        public ICosmosDbFunctionOptionBuilder<TCommand> ChangeFeedFunction<TCommand, TCosmosDbErrorHandler>(
            string collectionName,
            string databaseName,
            string leaseCollectionName = "leases",
            string leaseDatabaseName = null,
            bool createLeaseCollectionIfNotExists = false,
            bool startFromBeginning = false,
            bool convertToPascalCase = false,
            string leaseCollectionPrefix = null,
            int? maxItemsPerInvocation = null,
            int? feedPollDelay = null,
            int? leaseAcquireInterval = null,
            int? leaseExpirationInterval = null,
            int? leaseRenewInterval = null,
            int? checkpointFrequency = null,
            int? leasesCollectionThroughput = null,
            bool trackRemainingWork = false,
            string remainingWorkCronExpression = "*/5 * * * * *"
            ) where TCosmosDbErrorHandler : ICosmosDbErrorHandler
        {
            CosmosDbFunctionDefinition definition = new CosmosDbFunctionDefinition(typeof(TCommand))
            {
                ConnectionStringName = _connectionStringName,
                CollectionName = collectionName,
                DatabaseName = databaseName,
                LeaseConnectionStringName = _leaseConnectionStringName,
                LeaseCollectionName = leaseCollectionName,
                LeaseDatabaseName = leaseDatabaseName ?? databaseName,
                CreateLeaseCollectionIfNotExists = createLeaseCollectionIfNotExists,
                ConvertToPascalCase = convertToPascalCase,
                StartFromBeginning = startFromBeginning,
                LeaseCollectionPrefix = leaseCollectionPrefix,
                MaxItemsPerInvocation = maxItemsPerInvocation,
                FeedPollDelay = feedPollDelay,
                LeaseAcquireInterval = leaseAcquireInterval,
                LeaseExpirationInterval = leaseExpirationInterval,
                LeaseRenewInterval = leaseRenewInterval,
                CheckpointFrequency = checkpointFrequency,
                LeasesCollectionThroughput = leasesCollectionThroughput,
                ErrorHandlerType = typeof(TCosmosDbErrorHandler),
                ErrorHandlerTypeName = typeof(TCosmosDbErrorHandler).EvaluateType(),
                TrackRemainingWork = trackRemainingWork,
                RemainingWorkCronExpression = remainingWorkCronExpression
            };
            _functionDefinitions.Add(definition);
            return new CosmosDbFunctionOptionBuilder<TCommand>(_connectionStringSettingNames, this, definition);
        }
    }
}
