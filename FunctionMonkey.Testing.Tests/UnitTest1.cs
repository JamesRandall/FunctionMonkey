using System.Threading.Tasks;
using FunctionMonkey.Testing.Tests.Helpers;
using FunctionMonkey.Tests.Integration.Functions.Commands;
using FunctionMonkey.Tests.Integration.Functions.Commands.Model;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Xunit;

namespace FunctionMonkey.Testing.Tests
{
    public class HttpTestsShould : AbstractAcceptanceTest
    {
        [Fact]
        public async Task ReturnOkResult()
        {
            HttpResponse httpResponse = await ExecuteHttpAsync(new HttpCommandWithResultAndNoValidation
            {
                Value = 5
            });

            Assert.Equal(200, httpResponse.StatusCode);
            SimpleResponse result = httpResponse.Body.DeserializeObject<SimpleResponse>();
            Assert.Equal(1, result.Value);
            Assert.Equal("success", result.Message);
        }
    }
}
