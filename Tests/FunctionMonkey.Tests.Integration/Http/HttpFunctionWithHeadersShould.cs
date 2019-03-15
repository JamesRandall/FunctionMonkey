using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using FunctionMonkey.Tests.Integration.Http.Helpers;
using Xunit;

namespace FunctionMonkey.Tests.Integration.Http
{
    public class HttpFunctionWithHeadersShould : AbstractHttpFunctionTest
    {
        private const int Value = 92316;

        private const string Message = "this is some text to be echoed";

        private void ValidateEchoedResponse(SimpleResponse response)
        {
            Assert.Equal(Value, response.Value);
            Assert.Equal(Message, response.Message);
        }

        [Fact]
        public async Task BindHeadersToCommand()
        {
            SimpleResponse response = await Settings.Host
                .AppendPathSegment("headers")
                .WithHeader("x-value", Value)
                .WithHeader("x-message", Message)
                .GetJsonAsync<SimpleResponse>();

            ValidateEchoedResponse(response);
        }
    }
}
