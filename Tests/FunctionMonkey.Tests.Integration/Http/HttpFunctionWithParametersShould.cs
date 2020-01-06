using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Flurl.Http;
using Newtonsoft.Json;
using Xunit;

namespace FunctionMonkey.Tests.Integration.Http
{
    public class HttpFunctionWithParametersShould : AbstractHttpFunctionTest
    {
        private const int Value = 92316;

        private const string Message = "this is some text to be echoed";
        
        [Fact]
        public async Task ReturnGuidFromGETWithGuidTypeQueryParam()
        {
            Guid value = Guid.NewGuid();
            
            HttpResponseMessage response = await Settings.Host
                .AllowAnyHttpStatus()
                .AppendPathSegment("queryParameters")
                .AppendPathSegment("guidQueryParam")
                .SetQueryParam("value", value)
                .GetAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            // Not testing the message due to differences in ASP.Net Core and Functions.
            //string responseString = await response.Content.ReadAsStringAsync();
            //Guid responseGuid = JsonConvert.DeserializeObject<Guid>(responseString);
            //Assert.Equal(value, responseGuid);
        }
        
        [Fact]
        public async Task ReturnBadRequestForGETWithMismatchedGuidTypeQueryParam()
        {
            HttpResponseMessage response = await Settings.Host
                .AllowAnyHttpStatus()
                .AppendPathSegment("queryParameters")
                .AppendPathSegment("guidQueryParam")
                .SetQueryParam("value", "invalidguid")
                .GetAsync();

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            // Not testing the message due to differences in ASP.Net Core and Functions.
            //string errorMessage = await response.Content.ReadAsStringAsync();
            // we convert the case of the response because the idiomatic case of the properties is different
            // in the C# and F# models
            //Assert.Equal("Invalid type for query parameter Value".ToLower(), errorMessage.ToLower());
        }
        
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
            // Not testing the message due to differences in ASP.Net Core and Functions.
            //string errorMessage = await response.Content.ReadAsStringAsync();
            //Assert.Equal("Invalid type for route parameter value", errorMessage);
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
            // Not testing the message due to differences in ASP.Net Core and Functions.
            //string errorMessage = await response.Content.ReadAsStringAsync();
            // we convert the case of the response because the idiomatic case of the properties is different
            // in the C# and F# models
            //Assert.Equal("Invalid type for query parameter Value".ToLower(), errorMessage.ToLower());
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
            // Not testing the message due to differences in ASP.Net Core and Functions.
            //string errorMessage = await response.Content.ReadAsStringAsync();
            // we convert the case of the response because the idiomatic case of the properties is different
            // in the C# and F# models
            //Assert.Equal("Invalid type for query parameter NullableGuid".ToLower(), errorMessage.ToLower());
        }

        [Fact]
        public async Task ReturnSumOfArrayQueryParams()
        {
            string url = $"{Settings.Host}queryParameters/array?value=1&value=3&value=5";
                
                
            string response = await url.GetStringAsync();

            int sumResult = int.Parse(response);
            Assert.Equal(9, sumResult);
        }
        
        [Fact]
        public async Task ReturnSumOfReadonlyCollectionQueryParams()
        {
            string url = $"{Settings.Host}queryParameters/readonlyCollection?value=1&value=3&value=5";
                
                
            string response = await url.GetStringAsync();

            int sumResult = int.Parse(response);
            Assert.Equal(9, sumResult);
        }
        
        [Fact]
        public async Task ReturnSumOfListQueryParams()
        {
            string url = $"{Settings.Host}queryParameters/list?value=1&value=3&value=5";
                
                
            string response = await url.GetStringAsync();

            int sumResult = int.Parse(response);
            Assert.Equal(9, sumResult);
        }
        
        [Fact]
        public async Task ReturnSumOfStringListQueryParams()
        {
            string url = $"{Settings.Host}queryParameters/stringList?value=1&value=3&value=5";
                
                
            string response = await url.GetStringAsync();

            int sumResult = int.Parse(response);
            Assert.Equal(9, sumResult);
        }
        
        [Fact]
        public async Task ReturnSumOfEnumerableQueryParams()
        {
            string url = $"{Settings.Host}queryParameters/enumerable?value=1&value=3&value=5";
                
                
            string response = await url.GetStringAsync();

            int sumResult = int.Parse(response);
            Assert.Equal(9, sumResult);
        }
    }
}