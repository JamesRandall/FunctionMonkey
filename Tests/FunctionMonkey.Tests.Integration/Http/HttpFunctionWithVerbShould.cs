using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using FunctionMonkey.Tests.Integration.Http.Helpers;
using Xunit;

namespace FunctionMonkey.Tests.Integration.Http
{
    public class HttpFunctionWithVerbShould : AbstractHttpFunctionTest
    {
        private const int Value = 92316;

        private const string Message = "this is some text to be echoed";

        private void ValidateEchoedResponse(SimpleResponse response)
        {
            Assert.Equal(Value, response.Value);
            Assert.Equal(Message, response.Message);
        }

        [Fact]
        public async Task ReturnEchoedPayloadForGET()
        {
            SimpleResponse response = await Settings.Host
                .AppendPathSegment("verbs")
                .AppendPathSegment(Value)
                .SetQueryParam("message", Message)
                .GetJsonAsync<SimpleResponse>();

            ValidateEchoedResponse(response);
        }

        [Fact]
        public async Task ReturnEchoedPayloadForDELETE()
        {
            SimpleResponse response = await Settings.Host
                .AppendPathSegment("verbs")
                .AppendPathSegment(Value)
                .SetQueryParam("message", Message)
                .DeleteAsync()
                .ReceiveJson<SimpleResponse>();

            ValidateEchoedResponse(response);
        }

        [Fact]
        public async Task ReturnEchoedPayloadForPOST()
        {
            SimpleResponse response = await Settings.Host
                .AppendPathSegment("verbs")
                .PostJsonAsync(new
                {
                    Value,
                    Message
                })
                .ReceiveJson<SimpleResponse>();

            ValidateEchoedResponse(response);
        }

        [Fact]
        public async Task ReturnEchoedPayloadForPUT()
        {
            SimpleResponse response = await Settings.Host
                .AppendPathSegment("verbs")
                .PutJsonAsync(new
                {
                    Value,
                    Message
                })
                .ReceiveJson<SimpleResponse>();

            ValidateEchoedResponse(response);
        }

        [Fact]
        public async Task ReturnEchoedPayloadForPATCH()
        {
            SimpleResponse response = await Settings.Host
                .AppendPathSegment("verbs")
                .PatchJsonAsync(new
                {
                    Value,
                    Message
                })
                .ReceiveJson<SimpleResponse>();

            ValidateEchoedResponse(response);
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
        }
    }
}
