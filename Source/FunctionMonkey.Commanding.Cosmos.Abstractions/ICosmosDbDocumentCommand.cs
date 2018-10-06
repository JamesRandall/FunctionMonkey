using System;
using AzureFromTheTrenches.Commanding.Abstractions;
using Microsoft.Azure.Documents;

namespace FunctionMonkey.Commanding.Cosmos.Abstractions
{
    public interface ICosmosDbDocumentCommand : ICommand
    {
        Document Document { get; set; }
    }
}
