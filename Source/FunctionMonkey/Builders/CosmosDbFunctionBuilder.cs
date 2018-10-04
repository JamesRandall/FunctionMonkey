using System;
using System.Collections.Generic;
using System.Text;
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
            string leaseCollectionName = "leases") where TCommand : ICommandClaimsBinder
        {
            _functionDefinitions.Add(new CosmosDbFunctionDefinition(typeof(TCommand))
            {
                ConnectionStringName = _connectionStringName,
                CollectionName = collectionName,
                DatabaseName = databaseName,
                LeaseCollectionName = leaseCollectionName
            });
            return this;
        }
    }
}
