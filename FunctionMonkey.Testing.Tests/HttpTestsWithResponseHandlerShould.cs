using System.Threading.Tasks;
using FunctionMonkey.Testing.Tests.Helpers;
using FunctionMonkey.Tests.Integration.Common.Commands.HttpResponseShaping;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace FunctionMonkey.Testing.Tests
{
    public class HttpTestsWithResponseHandlerShould : AbstractAcceptanceTest
    {
        [Fact]
        public async Task ReturnOkResultForCommandWithResultWhenNoValidator()
        {
            HttpResponse httpResponse = await ExecuteHttpAsync(new HttpResponseHandlerCommandWithResultAndNoValidation());

            Assert.Equal(200, httpResponse.StatusCode);
            string transformedByHandlerResponse = httpResponse.Body.GetString();
            Assert.Equal("CreateResponse<TCommand,TResult>", transformedByHandlerResponse);
        }

        [Fact]
        public async Task ReturnOkResultForCommandWithResultWhenValidatorPasses()
        {
            HttpResponse httpResponse = await ExecuteHttpAsync(new HttpResponseHandlerCommandWithResultAndValidatorThatPasses());

            Assert.Equal(200, httpResponse.StatusCode);
            string transformedByHandlerResponse = httpResponse.Body.GetString();
            Assert.Equal("CreateResponse<TCommand,TResult>", transformedByHandlerResponse);
        }

        [Fact]
        public async Task ReturnOkResultForCommandWithResultWhenValidatorFails()
        {
            HttpResponse httpResponse = await ExecuteHttpAsync(new HttpResponseHandlerCommandWithResultAndValidatorThatFails());

            Assert.Equal(200, httpResponse.StatusCode);
            string transformedByHandlerResponse = httpResponse.Body.GetString();
            Assert.Equal("CreateValidationFailureResponse<TCommand>", transformedByHandlerResponse);
        }

        [Fact]
        public async Task ReturnOkResultForCommandWithNoResultWhenNoValidator()
        {
            HttpResponse httpResponse = await ExecuteHttpAsync(new HttpResponseHandlerCommandWithNoResultAndNoValidation
            {
                Value = 5
            });

            Assert.Equal(200, httpResponse.StatusCode);
            string transformedByHandlerResponse = httpResponse.Body.GetString();
            Assert.Equal("CreateResponse<TCommand>", transformedByHandlerResponse);
        }

        [Fact]
        public async Task ReturnOkResultForCommandWithNoResultWhenValidatorPasses()
        {
            HttpResponse httpResponse = await ExecuteHttpAsync(new HttpResponseHandlerCommandWithNoResultAndValidatorThatPasses());

            Assert.Equal(200, httpResponse.StatusCode);
            string transformedByHandlerResponse = httpResponse.Body.GetString();
            Assert.Equal("CreateResponse<TCommand>", transformedByHandlerResponse);
        }

        [Fact]
        public async Task ReturnOkResultForCommandWithNoResultWhenValidatorFails()
        {
            HttpResponse httpResponse = await ExecuteHttpAsync(new HttpResponseHandlerCommandWithNoResultAndValidatorThatFails());

            Assert.Equal(200, httpResponse.StatusCode);
            string transformedByHandlerResponse = httpResponse.Body.GetString();
            Assert.Equal("CreateValidationFailureResponse<TCommand>", transformedByHandlerResponse);
        }
    }
}
