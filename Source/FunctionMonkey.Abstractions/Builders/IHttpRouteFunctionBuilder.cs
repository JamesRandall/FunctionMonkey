using System;

namespace FunctionMonkey.Abstractions.Builders
{
    /// <summary>
    /// Used to optionally annotate a function with metadata for Open API
    /// </summary>
    public interface IHttpRouteFunctionBuilder : IFunctionBuilder
    {
        /// <summary>
        /// The Open API / Swagger description for the endpoint
        /// </summary>
        /// <param name="description">The description</param>
        /// <returns>A IHttpRouteFunctionBuilder that allows further functions to be created and this route to be further configured with Open API / Swagger metadata.</returns>
        IHttpRouteFunctionBuilder OpenApiDescription(string description);

        /// <summary>
        /// The Open API / Swagger name for the route
        /// </summary>
        /// <param name="name">The name</param>
        /// <returns>A IHttpRouteFunctionBuilder that allows further functions to be created and this route to be further configured with Open API / Swagger metadata.</returns>
        IHttpRouteFunctionBuilder OpenApiName(string name);

        /// <summary>
        /// Allow options to be configured for the route
        /// </summary>
        IHttpRouteFunctionBuilder Options(Action<IHttpRouteOptionsBuilder> options);
    }
}
