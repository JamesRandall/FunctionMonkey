using System;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Abstractions.Builders.Model;
using FunctionMonkey.Commanding.Cosmos.Abstractions;
using FunctionMonkey.Model;
using Newtonsoft.Json.Serialization;

namespace FunctionMonkey.Builders
{
    public class CosmosDbFunctionOptionBuilder<TCommandOuter> : ICosmosDbFunctionOptionBuilder<TCommandOuter>
    {
        private readonly ConnectionStringSettingNames _connectionStringSettingNames;
        private readonly ICosmosDbFunctionBuilder _underlyingBuilder;
        private readonly CosmosDbFunctionDefinition _functionDefinition;
        
        public CosmosDbFunctionOptionBuilder(
            ConnectionStringSettingNames connectionStringSettingNames,
            ICosmosDbFunctionBuilder underlyingBuilder,
            CosmosDbFunctionDefinition functionDefinition)
        {
            _connectionStringSettingNames = connectionStringSettingNames;
            _underlyingBuilder = underlyingBuilder;
            _functionDefinition = functionDefinition;
        }
        
        public ICosmosDbFunctionOptionBuilder<TCommand> ChangeFeedFunction<TCommand>(string collectionName, string databaseName,
            string leaseCollectionName = "leases", string leaseDatabaseName = null,
            bool createLeaseCollectionIfNotExists = false, bool startFromBeginning = false, bool convertToPascalCase = false,
            string leaseCollectionPrefix = null, int? maxItemsPerInvocation = null, int? feedPollDelay = null,
            int? leaseAcquireInterval = null, int? leaseExpirationInterval = null, int? leaseRenewInterval = null,
            int? checkpointFrequency = null, int? leasesCollectionThroughput = null,
            bool trackRemainingWork = true,
            string remainingWorkCronExpression = "*/1 * * * * *")
        {
            return _underlyingBuilder.ChangeFeedFunction<TCommand>(collectionName, databaseName,
                leaseCollectionName, leaseDatabaseName,
                createLeaseCollectionIfNotExists, startFromBeginning, convertToPascalCase,
                leaseCollectionPrefix, maxItemsPerInvocation, feedPollDelay,
                leaseAcquireInterval, leaseExpirationInterval, leaseRenewInterval,
                checkpointFrequency, leasesCollectionThroughput, trackRemainingWork, remainingWorkCronExpression);
        }

        public ICosmosDbFunctionOptionBuilder<TCommand> ChangeFeedFunction<TCommand, TCosmosDbErrorHandler>(string collectionName, string databaseName,
            string leaseCollectionName = "leases", string leaseDatabaseName = null,
            bool createLeaseCollectionIfNotExists = false, bool startFromBeginning = false, bool convertToPascalCase = false,
            string leaseCollectionPrefix = null, int? maxItemsPerInvocation = null, int? feedPollDelay = null,
            int? leaseAcquireInterval = null, int? leaseExpirationInterval = null, int? leaseRenewInterval = null,
            int? checkpointFrequency = null, int? leasesCollectionThroughput = null,
            bool trackRemainingWork = true,
            string remainingWorkCronExpression = "*/1 * * * * *") where TCosmosDbErrorHandler : ICosmosDbErrorHandler
        {
            return _underlyingBuilder.ChangeFeedFunction<TCommand, TCosmosDbErrorHandler>(collectionName, databaseName,
                leaseCollectionName, leaseDatabaseName,
                createLeaseCollectionIfNotExists, startFromBeginning, convertToPascalCase,
                leaseCollectionPrefix, maxItemsPerInvocation, feedPollDelay,
                leaseAcquireInterval, leaseExpirationInterval, leaseRenewInterval,
                checkpointFrequency, leasesCollectionThroughput, trackRemainingWork, remainingWorkCronExpression);
        }

        public ICosmosDbFunctionOptionBuilder<TCommandOuter> Options(Action<IFunctionOptionsBuilder> options)
        {
            FunctionOptionsBuilder builder = new FunctionOptionsBuilder(_functionDefinition);
            options(builder);
            return this;
        }

        public IOutputBindingBuilder<ICosmosDbFunctionOptionBuilder<TCommandOuter>> OutputTo =>
            new OutputBindingBuilder<ICosmosDbFunctionOptionBuilder<TCommandOuter>>(_connectionStringSettingNames, this, _functionDefinition, _pendingOutputConverterType);

        private Type _pendingOutputConverterType = null;
        public ICosmosDbFunctionOptionBuilder<TCommandOuter> OutputBindingConverter<TConverter>() where TConverter : IOutputBindingConverter
        {
            if (_functionDefinition.OutputBinding != null)
            {
                _functionDefinition.OutputBinding.OutputBindingConverterType = typeof(TConverter);
            }
            else
            {
                _pendingOutputConverterType = typeof(TConverter);
            }

            return this;
        }
    }
}