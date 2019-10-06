using System;
using System.Collections.Generic;
using System.Net.Http;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Abstractions.Builders.Model;
using FunctionMonkey.Abstractions.Extensions;
using FunctionMonkey.Extensions;

namespace FunctionMonkey.Model
{
    public class HttpFunctionDefinition : AbstractFunctionDefinition
    {
        public HttpFunctionDefinition(Type commandType) : base("", commandType)
        {
        }
        
        public HttpFunctionDefinition(Type commandType, Type explicitCommandResultType) : base("", commandType, explicitCommandResultType)
        {
        }
        
        public HashSet<HttpMethod> Verbs { get; set; } = new HashSet<HttpMethod>();

        public AuthorizationTypeEnum? Authorization { get; set; }

        public bool ValidatesToken { get; set; }

        public HttpRouteConfiguration RouteConfiguration { get; set; }

        public string Route { get; set; }

        public bool HasRoute => Route != null;

        public IReadOnlyCollection<HttpParameter> QueryParameters { get; set; }
        
        public IReadOnlyCollection<HttpParameter> PossibleFormProperties { get; set; }

        public IReadOnlyCollection<HttpParameter> RouteParameters { get; set; }

        public string OpenApiDescription { get; set; }

        public Dictionary<int, string> OpenApiResponseDescriptions { get; set; } = new Dictionary<int, string>();

        public string SubRoute { get; set; }

        public string TokenHeader { get; set; }

        public bool IsValidationResult { get; set; }
        
        public Type TokenValidatorType { get; set; }

        public string TokenValidatorTypeName => TokenValidatorType?.EvaluateType();

        public bool AuthorizesClaims => !string.IsNullOrWhiteSpace(ClaimsPrincipalAuthorizationTypeName);

        public Type ClaimsPrincipalAuthorizationType { get; set; }

        public string ClaimsPrincipalAuthorizationTypeName => ClaimsPrincipalAuthorizationType?.EvaluateType();

        public HeaderBindingConfiguration HeaderBindingConfiguration { get; set; }

        public bool HasHttpResponseHandler => 
            HttpResponseHandlerType != null ||
            CreateResponseFromExceptionFunction != null ||
            CreateResponseFunction != null ||
            CreateResponseForResultFunction != null ||
            CreateValidationFailureResponseFunction != null;

        public Type HttpResponseHandlerType { get; set; }

        public string HttpResponseHandlerTypeName => HttpResponseHandlerType.EvaluateType();

        public bool IsStreamCommand { get; set; }
        
        public bool ReturnResponseBodyWithOutputBinding { get; set; }
        
        // F# Support
        public BridgedFunction TokenValidatorFunction { get; set; }
        
        public BridgedFunction CreateResponseFromExceptionFunction { get; set; }
        
        public BridgedFunction CreateResponseFunction { get; set; }
        
        public BridgedFunction CreateResponseForResultFunction { get; set; }
        
        public BridgedFunction CreateValidationFailureResponseFunction { get; set; }
    }
}
