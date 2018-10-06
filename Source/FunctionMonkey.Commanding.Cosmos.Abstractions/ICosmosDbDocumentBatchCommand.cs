using System.Collections.Generic;
using AzureFromTheTrenches.Commanding.Abstractions;
using Microsoft.Azure.Documents;

namespace FunctionMonkey.Commanding.Cosmos.Abstractions
{
    public interface ICosmosDbDocumentBatchCommand : ICommand
    {
        IReadOnlyCollection<Document> Documents { get; set; }
    }
}
