using System;
using System.Collections.Generic;
using System.Text;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Commanding.Cosmos.Abstractions;
using Microsoft.Azure.Documents;

namespace SwaggerBuildOut.Commands
{
    public class CosmosDocumentCommand : ICosmosDbDocumentCommand
    {
        public Document Document { get; set; }
    }
}
