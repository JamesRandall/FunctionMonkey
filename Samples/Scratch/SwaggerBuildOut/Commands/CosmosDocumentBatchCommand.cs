using System.Collections.Generic;
using FunctionMonkey.Commanding.Cosmos.Abstractions;
using Microsoft.Azure.Documents;

namespace SwaggerBuildOut.Commands
{
    public class CosmosDocumentBatchCommand : ICosmosDbDocumentBatchCommand
    {
        public IReadOnlyCollection<Document> Documents { get; set; }
    }
}
