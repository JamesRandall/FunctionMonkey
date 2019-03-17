using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using FunctionMonkey.Tests.Integration.Http.Helpers;
using Xunit;

namespace FunctionMonkey.Tests.Integration.Http
{
    public class HttpFunctionWithRouteParameterShould : AbstractHttpFunctionTest
    {
        public const string Message = "requiredMessage";
        public const int Value = 23;
        public const int OptionalValue = 10;
        public const string OptionalMessage = "optionalMessage";

        [Fact]
        public async Task SetBothRequiredAndOptionalRouteParameters()
        {
            RouteParameterResponse response = await Settings.Host
                .AppendPathSegment("routeParameters")
                .AppendPathSegment(Message)
                .AppendPathSegment(Value)
                .AppendPathSegment(OptionalValue)
                .AppendPathSegment(OptionalMessage)
                .GetJsonAsync<RouteParameterResponse>();

            Assert.Equal(Message, response.Message);
            Assert.Equal(Value, response.Value);
            Assert.Equal(OptionalValue, response.OptionalValue);
            Assert.Equal(OptionalMessage, response.OptionalMessage);
        }

        [Fact]
        public async Task IgnoreOptionalRouteParametersWhenNotIncludedInUri()
        {
            RouteParameterResponse response = await Settings.Host
                .AppendPathSegment("routeParameters")
                .AppendPathSegment(Message)
                .AppendPathSegment(Value)
                .GetJsonAsync<RouteParameterResponse>();

            Assert.Equal(Message, response.Message);
            Assert.Equal(Value, response.Value);
            Assert.Null(response.OptionalValue);
            Assert.Null(response.OptionalMessage);
        }

        [Fact]
        public async Task SetFirstOptionalParametersWhenIncludedInUri()
        {
            RouteParameterResponse response = await Settings.Host
                .AppendPathSegment("routeParameters")
                .AppendPathSegment(Message)
                .AppendPathSegment(Value)
                .AppendPathSegment(OptionalValue)
                .GetJsonAsync<RouteParameterResponse>();

            Assert.Equal(Message, response.Message);
            Assert.Equal(Value, response.Value);
            Assert.Equal(OptionalValue, response.OptionalValue);
            Assert.Null(response.OptionalMessage);
        }

        [Fact]
        public async Task HandleGuidParametersInUri()
        {
            Guid guidOne = Guid.NewGuid();
            Guid guidTwo = Guid.NewGuid();
            // there is a bug in Azure Functions - using a GUID as a route parameter doesn't work
            // Function Monkey will work round this by accepting them as strings in the trigger
            // and parsing them internally
            GuidPairResponse response = await Settings.Host
                .AppendPathSegment("routeParameters")
                .AppendPathSegment("guids")
                .AppendPathSegment(guidOne)
                .AppendPathSegment(guidTwo)
                .GetJsonAsync<GuidPairResponse>();

            Assert.Equal(guidOne, response.ValueOne);
            Assert.Equal(guidTwo, response.ValueTwo);
        }

        [Fact]
        public async Task HandleOptionalGuidParameterInUri()
        {
            Guid guidOne = Guid.NewGuid();
            // there is a bug in Azure Functions - using a GUID as a route parameter doesn't work
            // Function Monkey will work round this by accepting them as strings in the trigger
            // and parsing them internally
            GuidPairResponse response = await Settings.Host
                .AppendPathSegment("routeParameters")
                .AppendPathSegment("guids")
                .AppendPathSegment(guidOne)
                .GetJsonAsync<GuidPairResponse>();

            Assert.Equal(guidOne, response.ValueOne);
            Assert.Null(response.ValueTwo);
        }

        [Fact]
        public async Task ReturnBadRequestOnBadlyFormedGuid()
        {
            // there is a bug in Azure Functions - using a GUID as a route parameter doesn't work
            // Function Monkey will work round this by accepting them as strings in the trigger
            // and parsing them internally
            HttpResponseMessage response = await Settings.Host
                .AppendPathSegment("routeParameters")
                .AppendPathSegment("guids")
                .AppendPathSegment("boo")
                .AllowHttpStatus(HttpStatusCode.BadRequest)
                .GetAsync();

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task ReturnBadRequestOnBadlyFormedOptionalGuid()
        {
            Guid guidOne = Guid.NewGuid();
            // there is a bug in Azure Functions - using a GUID as a route parameter doesn't work
            // Function Monkey will work round this by accepting them as strings in the trigger
            // and parsing them internally
            HttpResponseMessage response = await Settings.Host
                .AppendPathSegment("routeParameters")
                .AppendPathSegment("guids")
                .AppendPathSegment(guidOne)
                .AppendPathSegment("boo")
                .AllowHttpStatus(HttpStatusCode.BadRequest)
                .GetAsync();

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
