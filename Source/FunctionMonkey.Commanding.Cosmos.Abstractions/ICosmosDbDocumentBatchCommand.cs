using System.Collections.Generic;
using Microsoft.Azure.Documents;

namespace FunctionMonkey.Commanding.Cosmos.Abstractions
{
    public interface ICosmosDbDocumentBatchCommand
    {
        IReadOnlyCollection<Document> Documents { get; set; }
    }
}
