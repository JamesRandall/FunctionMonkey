using System;
using Microsoft.Azure.Documents;

namespace FunctionMonkey.Commanding.Cosmos.Abstractions
{
    public interface ICosmosDbDocumentCommand
    {
        Document Document { get; set; }
    }
}
