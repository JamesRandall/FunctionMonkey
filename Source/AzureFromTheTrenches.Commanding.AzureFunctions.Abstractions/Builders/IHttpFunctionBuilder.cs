using System;
using System.Net.Http;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.AzureFunctions.Abstractions.Builders
{
    public interface IHttpFunctionBuilder
    {
        IHttpFunctionBuilderMetadataBuilder HttpFunction<TCommand>() where TCommand : ICommand;
        IHttpFunctionBuilderMetadataBuilder HttpFunction<TCommand>(params HttpMethod[] method) where TCommand : ICommand;
        IHttpFunctionBuilderMetadataBuilder HttpFunction<TCommand>(string route, params HttpMethod[] method) where TCommand : ICommand;
    }
}
