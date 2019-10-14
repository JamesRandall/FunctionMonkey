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
        
        [Fact]
        public async Task BindHeaderToCommandWithDefaultMapping()
        {
            SimpleResponse response = await Settings.Host
                .AppendPathSegment("headers")
                .AppendPathSegment("default")
                .WithHeader("x-default-int", Value)
                .WithHeader("x-default-string", Message)
                .GetJsonAsync<SimpleResponse>();

            ValidateEchoedResponse(response);
        }
        
        [Fact]
        public async Task BindHeaderToNullableValueType()
        {
            SimpleResponse response = await Settings.Host
                .AppendPathSegment("headers/nullableValueType")
                .WithHeader("x-value", Value)
                .GetJsonAsync<SimpleResponse>();

            Assert.Equal(Value, response.Value);
        }
        
        [Fact]
        public async Task BindHeaderToEnumType()
        {
            SimpleResponse response = await Settings.Host
                .AppendPathSegment("headers/enumType")
                .WithHeader("x-enum-value", "AnotherValue")
                .GetJsonAsync<SimpleResponse>();

            Assert.Equal(Value, response.Value);
        }
    }
}
