using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionMonkey.Model
{
    public class CosmosDbFunctionDefinition : AbstractFunctionDefinition
    {
        public CosmosDbFunctionDefinition(Type commandType) : base("CosmosFn", commandType)
        {
        }

        public string DatabaseName { get; set; }

        public string CollectionName { get; set; }

        public string ConnectionStringName { get; set; }

        public IReadOnlyCollection<CosmosDbCommandProperty> CommandProperties { get; set; }

        public bool IsDocumentCommand { get; set; }

        public bool IsDocumentBatchCommand { get; set; }

        public string LeaseCollectionName { get; set; }

        public string LeaseDatabaseName { get; set; }

        public bool DocumentIsCamelCase { get; set; }

        public bool CreateLeaseCollectionIfNotExists { get; set; }
    }
}
