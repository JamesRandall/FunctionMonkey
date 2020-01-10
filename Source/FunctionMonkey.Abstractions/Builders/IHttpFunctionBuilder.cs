using System.Net.Http;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions.Builders.Model;

namespace FunctionMonkey.Abstractions.Builders
{
    /// <summary>
    /// Interface that allows for the configuration of HTTP triggered functions
    /// </summary>
    public interface IHttpFunctionBuilder
    {        
        /// <summary>
        /// Associate a GET verb function with the given command. Its endpoint will be that specified by the parent HTTP route and with the default authorization type (see IAuthorizationBuilder)
        /// </summary>
        /// <typeparam name="TCommand">The command type</typeparam>
        /// <returns>A IHttpFunctionConfigurationBuilder that allows further HTTP functions to be created and this function to be further configured with Open API / Swagger metadata.</returns>
        IHttpFunctionConfigurationBuilder<TCommand> HttpFunction<TCommand>();

        /// <summary>
        /// Associate a GET verb function with the given command type and authorization type. Its endpoint will be that specified by the parent HTTP route.
        /// </summary>
        /// <typeparam name="TCommand">The command type</typeparam>
        /// <param name="authorizationType">The authorization type of the function</param>
        /// <returns>A IHttpFunctionConfigurationBuilder that allows further HTTP functions to be created and this function to be further configured with Open API / Swagger metadata.</returns>
        IHttpFunctionConfigurationBuilder<TCommand> HttpFunction<TCommand>(AuthorizationTypeEnum authorizationType);

        /// <summary>
        /// Associate a function with the given command type, authorization type and HTTP verbs. Its endpoint will be that specified by the parent HTTP route.
        /// </summary>
        /// <typeparam name="TCommand">The command type</typeparam>
        /// <param name="authorizationType">The authorization type of the function</param>
        /// <param name="method">One or more HTTP methods to support.</param>
        /// <returns>A IHttpFunctionConfigurationBuilder that allows further HTTP functions to be created and this function to be further configured with Open API / Swagger metadata.</returns>
        IHttpFunctionConfigurationBuilder<TCommand> HttpFunction<TCommand>(AuthorizationTypeEnum authorizationType, params HttpMethod[] method);

        /// <summary>
        /// Assocate a function with the given command type and associate it with the specified verbs. Its endpoint will be that specified by the parent HTTP route and with the default authorization type (see IAuthorizationBuilder)
        /// </summary>
        /// <param name="method">One or more HTTP methods to support.</param>
        /// <returns>A IHttpFunctionConfigurationBuilder that allows further HTTP functions to be created and this function to be further configured with Open API / Swagger metadata.</returns>
        IHttpFunctionConfigurationBuilder<TCommand> HttpFunction<TCommand>(params HttpMethod[] method);

        /// <summary>
        /// Assocate a function with the given command type and associate it with the specified verbs. Its endpoint will be that specified by the parent HTTP route concatenated with the parent route 
        /// specified here and it will be secured with the default authorization type (see IAuthorizationBuilder)
        /// </summary>
        /// <typeparam name="TCommand">The command type</typeparam>
        /// <param name="route">The route to concatenate with the parent route</param>
        /// <param name="method">One or more HTTP methods to support.</param>
        /// <returns>A IHttpFunctionConfigurationBuilder that allows further HTTP functions to be created and this function to be further configured with Open API / Swagger metadata.</returns>
        IHttpFunctionConfigurationBuilder<TCommand> HttpFunction<TCommand>(string route, params HttpMethod[] method);

        /// <summary>
        /// Assocate a function with the given command type and associate it with the specified verbs. Its endpoint will be that specified by the parent HTTP route concatenated with the parent route 
        /// specified here and it will be secured by the specified authorization type.
        /// </summary>
        /// <typeparam name="TCommand">The command type</typeparam>
        /// <param name="route">The route to concatenate with the parent route</param>
        /// <param name="authorizationType">The authorization type of the function</param>
        /// <param name="method">One or more HTTP methods to support.</param>
        /// <returns>A IHttpFunctionConfigurationBuilder that allows further HTTP functions to be created and this function to be further configured with Open API / Swagger metadata.</returns>
        IHttpFunctionConfigurationBuilder<TCommand> HttpFunction<TCommand>(string route, AuthorizationTypeEnum authorizationType, params HttpMethod[] method);
    }
}
