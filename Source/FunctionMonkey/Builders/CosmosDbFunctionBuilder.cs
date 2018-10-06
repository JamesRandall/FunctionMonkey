using System;
using System.Collections.Generic;
using System.Text;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Model;

namespace FunctionMonkey.Builders
{
    internal class CosmosDbFunctionBuilder : ICosmosDbFunctionBuilder
    {
        private readonly string _connectionStringName;
        private readonly string _leaseConnectionStringName;
        private readonly List<AbstractFunctionDefinition> _functionDefinitions;

        public CosmosDbFunctionBuilder(string connectionStringName,
            string leaseConnectionName,
            List<AbstractFunctionDefinition> functionDefinitions)
        {
            _connectionStringName = connectionStringName;
            _functionDefinitions = functionDefinitions;
            _leaseConnectionStringName = leaseConnectionName;
        }

        public ICosmosDbFunctionBuilder ChangeFeedFunction<TCommand>(
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
            int? leasesCollectionThroughput = null
            ) where TCommand : ICommand
        {
            _functionDefinitions.Add(new CosmosDbFunctionDefinition(typeof(TCommand))
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
                LeasesCollectionThroughput = leasesCollectionThroughput
            });
            return this;
        }
    }
}
