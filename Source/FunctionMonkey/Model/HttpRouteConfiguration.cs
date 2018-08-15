using System;

namespace FunctionMonkey.Model
{
    public class HttpRouteConfiguration
    {
        public string OpenApiName { get; set; }

        public string OpenApiDescription { get; set; }

        public string Route { get; set; }

        public Type ClaimsPrincipalAuthorizationType { get; set; }
    }
}
