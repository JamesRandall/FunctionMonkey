using System;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Commanding.Cosmos.Abstractions;
using FunctionMonkey.Model;
using Newtonsoft.Json.Serialization;

namespace FunctionMonkey.Builders
{
    public class CosmosDbFunctionOptionBuilder : ICosmosDbFunctionOptionBuilder
    {
        private readonly ICosmosDbFunctionBuilder _underlyingBuilder;
        private readonly CosmosDbFunctionDefinition _functionDefinition;

        public CosmosDbFunctionOptionBuilder(ICosmosDbFunctionBuilder underlyingBuilder,
            CosmosDbFunctionDefinition functionDefinition)
        {
            _underlyingBuilder = underlyingBuilder;
            _functionDefinition = functionDefinition;
        }
        
        public ICosmosDbFunctionOptionBuilder ChangeFeedFunction<TCommand>(string collectionName, string databaseName,
            string leaseCollectionName = "leases", string leaseDatabaseName = null,
            bool createLeaseCollectionIfNotExists = false, bool startFromBeginning = false, bool convertToPascalCase = false,
            string leaseCollectionPrefix = null, int? maxItemsPerInvocation = null, int? feedPollDelay = null,
            int? leaseAcquireInterval = null, int? leaseExpirationInterval = null, int? leaseRenewInterval = null,
            int? checkpointFrequency = null, int? leasesCollectionThroughput = null,
            bool trackRemainingWork = true,
            string remainingWorkCronExpression = "*/1 * * * * *") where TCommand : ICommand
        {
            return _underlyingBuilder.ChangeFeedFunction<TCommand>(collectionName, databaseName,
                leaseCollectionName, leaseDatabaseName,
                createLeaseCollectionIfNotExists, startFromBeginning, convertToPascalCase,
                leaseCollectionPrefix, maxItemsPerInvocation, feedPollDelay,
                leaseAcquireInterval, leaseExpirationInterval, leaseRenewInterval,
                checkpointFrequency, leasesCollectionThroughput, trackRemainingWork, remainingWorkCronExpression);
        }

        public ICosmosDbFunctionOptionBuilder ChangeFeedFunction<TCommand, TCosmosDbErrorHandler>(string collectionName, string databaseName,
            string leaseCollectionName = "leases", string leaseDatabaseName = null,
            bool createLeaseCollectionIfNotExists = false, bool startFromBeginning = false, bool convertToPascalCase = false,
            string leaseCollectionPrefix = null, int? maxItemsPerInvocation = null, int? feedPollDelay = null,
            int? leaseAcquireInterval = null, int? leaseExpirationInterval = null, int? leaseRenewInterval = null,
            int? checkpointFrequency = null, int? leasesCollectionThroughput = null,
            bool trackRemainingWork = true,
            string remainingWorkCronExpression = "*/1 * * * * *") where TCommand : ICommand where TCosmosDbErrorHandler : ICosmosDbErrorHandler
        {
            return _underlyingBuilder.ChangeFeedFunction<TCommand, TCosmosDbErrorHandler>(collectionName, databaseName,
                leaseCollectionName, leaseDatabaseName,
                createLeaseCollectionIfNotExists, startFromBeginning, convertToPascalCase,
                leaseCollectionPrefix, maxItemsPerInvocation, feedPollDelay,
                leaseAcquireInterval, leaseExpirationInterval, leaseRenewInterval,
                checkpointFrequency, leasesCollectionThroughput, trackRemainingWork, remainingWorkCronExpression);
        }

        public ICosmosDbFunctionOptionBuilder Options(Action<IFunctionOptionsBuilder> options)
        {
            FunctionOptionsBuilder builder = new FunctionOptionsBuilder(_functionDefinition);
            options(builder);
            return this;
        }
    }
}