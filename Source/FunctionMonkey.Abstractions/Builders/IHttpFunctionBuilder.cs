using System.Net.Http;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Abstractions.Builders
{
    public interface IHttpFunctionBuilder
    {
        IHttpFunctionBuilderMetadataBuilder HttpFunction<TCommand>() where TCommand : ICommand;
        IHttpFunctionBuilderMetadataBuilder HttpFunction<TCommand>(params HttpMethod[] method) where TCommand : ICommand;
        IHttpFunctionBuilderMetadataBuilder HttpFunction<TCommand>(string route, params HttpMethod[] method) where TCommand : ICommand;
    }
}
