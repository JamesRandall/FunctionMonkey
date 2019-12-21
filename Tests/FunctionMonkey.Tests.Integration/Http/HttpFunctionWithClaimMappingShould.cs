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
    public class HttpFunctionWithClaimMappingShould : AbstractHttpFunctionTest
    {
        [Fact]
        public async Task ReturnMappedStringClaim()
        {
            SimpleResponse response = await Settings.Host
                .AppendPathSegment("claims")
                .AppendPathSegment("string")
                .WithOAuthBearerToken("some token")
                .GetJsonAsync<SimpleResponse>();

            Assert.Equal("a message", response.Message);
        }

        [Fact]
        public async Task ReturnMappedIntClaim()
        {
            SimpleResponse response = await Settings.Host
                .AppendPathSegment("claims")
                .AppendPathSegment("int")
                .WithOAuthBearerToken("some token")
                .GetJsonAsync<SimpleResponse>();

            Assert.Equal(42, response.Value);
        }
    }
}
