using System;

namespace FunctionMonkey.Abstractions.Builders
{
    /// <summary>
    /// An interface that allows functions to be configured during the building process
    /// </summary>
    public interface IFunctionBuilder
    {
        /// <summary>
        /// Create a route for one or more HTTP triggered functions
        /// </summary>
        /// <param name="routePrefix">The route - e.g. /api/v1/invoice</param>
        /// <param name="httpFunctionBuilder">The builder function for creating functions under this route</param>
        /// <returns>The function builder for a fluent API</returns>
        IFunctionBuilder HttpRoute(string routePrefix, Action<IHttpFunctionBuilder> httpFunctionBuilder);

        /// <summary>
        /// Create a route for one or more HTTP triggered functions
        /// </summary>
        /// <param name="routePrefix">The route - e.g. /api/v1/invoice</param>
        /// <param name="openApiName">The name to give the route in the OpenAPI / Swagger definition e.g. Invoice</param>
        /// <param name="httpFunctionBuilder">The builder function for creating functions under this route</param>
        /// <returns>The function builder for a fluent API</returns>
        IFunctionBuilder HttpRoute(string routePrefix, string openApiName, Action<IHttpFunctionBuilder> httpFunctionBuilder);

        /// <summary>
        /// Create a route for one or more HTTP triggered functions
        /// </summary>
        /// <param name="routePrefix">The route - e.g. /api/v1/invoice</param>
        /// <param name="openApiName">The name to give the route in the OpenAPI / Swagger definition e.g. Invoice</param>
        /// <param name="openApiDescription">The description to give the route in the Open API / Swagger definition</param>
        /// <param name="httpFunctionBuilder">The builder function for creating functions under this route</param>
        /// <returns>The function builder for a fluent API</returns>
        IFunctionBuilder HttpRoute(string routePrefix, string openApiName, string openApiDescription, Action<IHttpFunctionBuilder> httpFunctionBuilder);

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
    }
}
