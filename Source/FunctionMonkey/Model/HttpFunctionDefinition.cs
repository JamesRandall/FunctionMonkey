using System;
using System.Collections.Generic;
using System.Net.Http;
using AzureFromTheTrenches.Commanding.AzureFunctions.Abstractions.Builders;

namespace AzureFromTheTrenches.Commanding.AzureFunctions.Model
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

        public IReadOnlyCollection<HttpQueryParameter> AcceptsQueryParameters { get; set; }

        public string OpenApiDescription { get; set; }

        public Dictionary<int, string> OpenApiResponseDescriptions { get; set; } = new Dictionary<int, string>();

        public string SubRoute { get; set; }
    }
}
