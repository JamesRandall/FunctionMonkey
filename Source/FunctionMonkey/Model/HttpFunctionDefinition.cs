using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Abstractions.Builders.Model;
using FunctionMonkey.Abstractions.Extensions;
using FunctionMonkey.Extensions;

namespace FunctionMonkey.Model
{
    public class HttpFunctionDefinition : AbstractFunctionDefinition
    {
        public HttpFunctionDefinition(Type commandType) : base("Http", commandType)
        {
        }
        
        public HttpFunctionDefinition(Type commandType, Type explicitCommandResultType) : base("Http", commandType, explicitCommandResultType)
        {
        }

        // This is used to determine if the command requires a body on an ASP.Net controller
        public bool CommandRequiresBody => CommandType.GetProperties().Length > 0;

        public bool HasQueryParametersWithoutHeaderMappingAndRouteParameters => QueryParametersWithoutHeaderMapping.Any() && RouteParameters.Any();

        public bool HasRouteParameters => RouteParameters.Any();
        
        public HashSet<HttpMethod> Verbs { get; set; } = new HashSet<HttpMethod>();

        public AuthorizationTypeEnum? Authorization { get; set; }

        public bool ValidatesToken { get; set; }

        public HttpRouteConfiguration RouteConfiguration { get; set; }

        public string Route { get; set; }

        public bool HasRoute => Route != null;

        public IReadOnlyCollection<HttpParameter> QueryParameters { get; set; }

        public IReadOnlyCollection<HttpParameter> QueryParametersWithHeaderMapping =>
            QueryParameters.Where(x => x.HasHeaderMapping).ToArray();
        
        public IReadOnlyCollection<HttpParameter> QueryParametersWithoutHeaderMapping =>
            QueryParameters.Where(x => !x.HasHeaderMapping).ToArray();
        
        public IReadOnlyCollection<HttpParameter> PossibleFormProperties { get; set; }

        public IReadOnlyCollection<HttpParameter> RouteParameters { get; set; }

        public string OpenApiDescription { get; set; }

        public string OpenApiSummary { get; set; }

        public Dictionary<int, OpenApiResponseConfiguration> OpenApiResponseConfigurations { get; set; } = new Dictionary<int, OpenApiResponseConfiguration>();

        public string SubRoute { get; set; }

        public string TokenHeader { get; set; }

        public bool IsValidationResult { get; set; }
        
        public Type TokenValidatorType { get; set; }

        public bool IsBodyBased =>
            !(Verbs.Contains(HttpMethod.Get) || Verbs.Contains(HttpMethod.Delete) || Verbs.Count == 0);

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
