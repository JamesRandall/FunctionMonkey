using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Commanding.Abstractions;

namespace FunctionMonkey.Abstractions.Builders
{
    public interface ISignalRFunctionBuilder
    {
        // TODO: Add a check in the assembvly compile type checker to make sure that Negotiate<TCommand> type commands have a return type of SignalRNegotiateResponse
        ISignalRFunctionConfigurationBuilder<TCommand> Negotiate<TCommand>(string route, AuthorizationTypeEnum? authorizationType  = null, params HttpMethod[] method);

        /// <summary>
        /// Creates a SignalR negotiator at the specified route attached to the given hub name and, optionally, with a user ID sourced using a binding expression e.g.
        /// {headers.x-ms-client-principal-id} to use the ID passed in from App Service Authentication (EasyAuth).
        /// </summary>
        /// <param name="route">The route</param>
        /// <param name="userIdMapping">Optional user ID mapping</param>
        /// <param name="authorizationType">The authorization type to use</param>
        /// <param name="method">The HTTP methods to bind to</param>
        /// <param name="hubName">The hub name</param>
        /// <returns></returns>
        ISignalRFunctionBuilder Negotiate(string route, string hubName, string userIdExpression = null, AuthorizationTypeEnum? authorizationType = null, params HttpMethod[] method);
        
        /// <summary>
        /// Creates a SignalR negotiator at the specified route attached to the given hub name that will source the user ID from the specified claim.
        /// Claim based negotiators always use token based authorization.
        /// </summary>
        /// <param name="route">The route for the negotiator</param>
        /// <param name="hubName">The name of the hub</param>
        /// <param name="claimType">The type of claim to use for a user ID</param>
        /// <param name="method">The HTTP methods to bind to</param>
        /// <returns></returns>
        ISignalRFunctionBuilder NegotiateWithClaim(string route, string hubName, string claimType, params HttpMethod[] method);
    }
}
