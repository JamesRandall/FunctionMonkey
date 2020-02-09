using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Xunit;

namespace FunctionMonkey.Tests.Integration.Http
{
    public class HttpFunctionWithNoRouteShould : AbstractHttpFunctionTest
    {
        [Fact]
        public async Task Return200()
        {
            HttpResponseMessage response = await Settings.Host
                .AppendPathSegment("HttpHttpCommandWithNoRoute")
                .GetAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
