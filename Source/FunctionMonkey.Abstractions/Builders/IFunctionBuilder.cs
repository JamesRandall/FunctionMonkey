using System;

namespace FunctionMonkey.Abstractions.Builders
{
    /// <summary>
    /// An interface that allows functions to be configured during the building process
    /// </summary>
    public interface IFunctionBuilder : ITimerFunctionBuilder
    {
        /// <summary>
        /// Create a route for one or more HTTP triggered functions
        /// </summary>
        /// <param name="routePrefix">The route - e.g. /api/v1/invoice</param>
        /// <param name="httpFunctionBuilder">The builder function for creating functions under this route</param>
        /// <returns>The function builder for a fluent API, additionally contains options for configuring the route with OpenAPI info</returns>
        IHttpRouteFunctionBuilder HttpRoute(string routePrefix, Action<IHttpFunctionBuilder> httpFunctionBuilder);

        /// <summary>
        /// Create a route for one or more HTTP triggered functions
        /// </summary>
        /// <param name="routePrefix">The route - e.g. /api/v1/invoice</param>
        /// <param name="httpFunctionBuilder">The builder function for creating functions under this route</param>
        /// <returns>The function builder for a fluent API, additionally contains options for configuring the route with OpenAPI info</returns>
        IHttpRouteFunctionBuilder HttpRoute<TAuthorizationType>(string routePrefix, Action<IHttpFunctionBuilder> httpFunctionBuilder) where TAuthorizationType : IClaimsPrincipalAuthorization;

        /// <summary>
        /// Allows Service Bus functions to be configured based on a connection name
        /// </summary>
        /// <param name="connectionName">The name of the connection in the environment settings</param>
        /// <param name="serviceBusFunctionBuilder">A builder that allows one or more functions to be created that are associated with this connection</param>
        /// <returns>The function builder for a fluent API</returns>
        IFunctionBuilder ServiceBus(string connectionName, Action<IServiceBusFunctionBuilder> serviceBusFunctionBuilder);

        /// <summary>
        /// Allows Azure Storage functions to be configured based on a connection name
        /// </summary>
        /// <param name="connectionName">The name of the connection in the environment settings</param>
        /// <param name="storageFunctionBuilder">A builder that allows one or more functions to be created that are associated with this connection</param>
        /// <returns>The function builder for a fluent API</returns>
        IFunctionBuilder Storage(string connectionName, Action<IStorageFunctionBuilder> storageFunctionBuilder);

        /// <summary>
        /// Allows Cosmos DB functions to be configured based on a connection name
        /// </summary>
        /// <param name="connectionName">The name of the connection in the environment settings</param>
        /// <param name="cosmosDbFunctionBuilder">A builder that allows one or more functions to be created that are associated with the Cosmos connection</param>
        /// <returns>The function builder for a fluent API</returns>
        IFunctionBuilder CosmosDb(string connectionName, Action<ICosmosDbFunctionBuilder> cosmosDbFunctionBuilder);

        /// <summary>
        /// Allows Cosmos DB functions to be configured based on a connection name
        /// </summary>
        /// <param name="connectionName">The name of the connection in the environment settings</param>
        /// <param name="leaseConnectionName">The name of the connection for the lease collection in the environment settings</param>
        /// <param name="cosmosDbFunctionBuilder">A builder that allows one or more functions to be created that are associated with the Cosmos connection</param>
        /// <returns>The function builder for a fluent API</returns>
        IFunctionBuilder CosmosDb(string connectionName, string leaseConnectionName, Action<ICosmosDbFunctionBuilder> cosmosDbFunctionBuilder);
    }
}
