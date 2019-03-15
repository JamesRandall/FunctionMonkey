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
    }
}
