using System;

namespace FunctionMonkey.Abstractions.Builders
{
    public interface IFunctionBuilder
    {
        IFunctionBuilder HttpRoute(string routePrefix, Action<IHttpFunctionBuilder> httpRouteBuilder);
        IFunctionBuilder HttpRoute(string routePrefix, string openApiName, Action<IHttpFunctionBuilder> httpRouteBuilder);
        IFunctionBuilder HttpRoute(string routePrefix, string openApiName, string openApiDescription, Action<IHttpFunctionBuilder> httpFunctionBuilder);

        IFunctionBuilder ServiceBus(string connectionName, Action<IServiceBusFunctionBuilder> serviceBusFunctionBuilder);

        /*IFunctionBuilder StorageQueueFunction<TCommand>() where TCommand : ICommand;
        IFunctionBuilder StorageQueueFunction<TCommand>(string functionName) where TCommand : ICommand;
        IFunctionBuilder StorageQueueFunction<TCommand>(Action<IStorageQueueFunctionBuilder> storageQueueFunctionBuilder) where TCommand : ICommand;
        IFunctionBuilder StorageQueueFunction<TCommand>(string functionName, Action<IStorageQueueFunctionBuilder> storageQueueFunctionBuilder) where TCommand : ICommand;*/
    }
}
