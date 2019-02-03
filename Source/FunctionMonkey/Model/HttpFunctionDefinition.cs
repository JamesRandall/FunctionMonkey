using System;
using System.Collections.Generic;
using System.Net.Http;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Abstractions.Builders.Model;
using FunctionMonkey.Extensions;

namespace FunctionMonkey.Model
{
    public class HttpFunctionDefinition : AbstractFunctionDefinition
    {
        // We want these to have nice routes so we don't apply the name prefix - every other type does
        public HttpFunctionDefinition(Type commandType) : base("", commandType)
        {
        }

        public HashSet<HttpMethod> Verbs { get; set; } = new HashSet<HttpMethod>();

        public AuthorizationTypeEnum? Authorization { get; set; }

        public bool ValidatesToken { get; set; }

        public HttpRouteConfiguration RouteConfiguration { get; set; }

        // used to create a proxy that maps through to the internal function
        public string Route { get; set; }

        public IReadOnlyCollection<HttpParameter> PossibleBindingProperties { get; set; }
        
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

        public bool HasHttpResponseHandler => HttpResponseHandlerType != null;

        public Type HttpResponseHandlerType { get; set; }

        public string HttpResponseHandlerTypeName => HttpResponseHandlerType.EvaluateType();

        public bool IsStreamCommand { get; set; }
    }
}
