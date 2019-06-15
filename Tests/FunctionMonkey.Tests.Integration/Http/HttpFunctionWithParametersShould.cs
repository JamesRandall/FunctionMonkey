using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Flurl.Http;
using Xunit;

namespace FunctionMonkey.Tests.Integration.Http
{
    public class HttpFunctionWithParametersShould : AbstractHttpFunctionTest
    {
        private const int Value = 92316;

        private const string Message = "this is some text to be echoed";
        
        [Fact]
        public async Task ReturnBadRequestForGETWithMismatchedTypeRouteParam()
        {
            HttpResponseMessage response = await Settings.Host
                .AllowAnyHttpStatus()
                .AppendPathSegment("verbs")
                .AppendPathSegment("mismatch")
                .SetQueryParam("message", Message)
                .GetAsync();

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            string errorMessage = await response.Content.ReadAsStringAsync();
            Assert.Equal("Invalid type for route parameter value", errorMessage);
        }
        
        [Fact]
        public async Task ReturnBadRequestForGETWithMismatchedTypeQueryParam()
        {
            HttpResponseMessage response = await Settings.Host
                .AllowAnyHttpStatus()
                .AppendPathSegment("queryParameters")
                .SetQueryParam("value", "abd")
                .GetAsync();

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            string errorMessage = await response.Content.ReadAsStringAsync();
            Assert.Equal("Invalid type for query parameter Value", errorMessage);
        }
        
        [Fact]
        public async Task ReturnBadRequestForNonNullableGuidWithMismatchedTypeQueryParam()
        {
            HttpResponseMessage response = await Settings.Host
                .AllowAnyHttpStatus()
                .AppendPathSegment("queryParameters")
                .SetQueryParam("value", 123)
                .SetQueryParam("nullableGuid", "abc")
                .GetAsync();

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            string errorMessage = await response.Content.ReadAsStringAsync();
            Assert.Equal("Invalid type for query parameter NullableGuid", errorMessage);
        }
    }
}