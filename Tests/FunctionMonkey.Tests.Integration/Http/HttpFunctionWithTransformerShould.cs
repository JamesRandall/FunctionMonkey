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
    public class HttpFunctionWithTransformerShould : AbstractHttpFunctionTest
    {
        private const int Value = 92316;

        private const string Message = "this is some text to be echoed";

        private void ValidateEchoedResponse(SimpleResponse response)
        {
            
        }

        [Fact]
        public async Task ReturnEchoedPayloadForGET()
        {
            SimpleResponse response = await Settings.Host
                .AppendPathSegment("transformer")
                .AppendPathSegment(Value)
                .SetQueryParam("message", Message)
                .GetJsonAsync<SimpleResponse>();

            Assert.Equal(Value+1, response.Value);
            Assert.Equal(Message, response.Message);
        }

        
    }
}
