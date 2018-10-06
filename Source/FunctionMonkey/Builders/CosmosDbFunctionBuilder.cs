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
        private readonly List<AbstractFunctionDefinition> _functionDefinitions;

        public CosmosDbFunctionBuilder(string connectionStringName,
            List<AbstractFunctionDefinition> functionDefinitions)
        {
            _connectionStringName = connectionStringName;
            _functionDefinitions = functionDefinitions;
        }

        public ICosmosDbFunctionBuilder ChangeFeedFunction<TCommand>(
            string collectionName,
            string databaseName,
            string leaseCollectionName = "leases",
            string leaseDatabaseName = null,
            bool createLeaseCollectionIfNotExists = false,
            bool startFromBeginning = false,
            bool convertToPascalCase = true) where TCommand : ICommand
        {
            _functionDefinitions.Add(new CosmosDbFunctionDefinition(typeof(TCommand))
            {
                ConnectionStringName = _connectionStringName,
                CollectionName = collectionName,
                DatabaseName = databaseName,
                LeaseCollectionName = leaseCollectionName,
                LeaseDatabaseName = leaseDatabaseName ?? databaseName,
                CreateLeaseCollectionIfNotExists = createLeaseCollectionIfNotExists,
                ConvertToPascalCase = convertToPascalCase,
                StartFromBeginning = startFromBeginning
            });
            return this;
        }
    }
}
