using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using FunctionMonkey.Tests.Integration.Http.Helpers;
using Newtonsoft.Json;
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
        public async Task ReturnEchoedPayloadForByteArrayPOST()
        {
            ByteResponse response = await Settings.Host
                .AppendPathSegment("verbs")
                .AppendPathSegment("bytes")
                .PostJsonAsync(new
                {
                    Bytes = Encoding.UTF8.GetBytes(Message)
                })
                .ReceiveJson<ByteResponse>();

            string message = Encoding.UTF8.GetString(response.Bytes);
            Assert.Equal(message, Message);
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
        public async Task ReturnBadRequestOnTypeMismtachForPOST()
        {
            HttpResponseMessage response = await Settings.Host
                .AllowAnyHttpStatus()
                .AppendPathSegment("verbs")
                .PostJsonAsync(new
                {
                    Value="mismatchedType",
                    Message
                });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            // ASP.Net Core returns a different error string for this
            //string responseString = await response.Content.ReadAsStringAsync();
            //Assert.Equal("Invalid type in message body at line 1 for path Value", responseString);
        }
    }
}
