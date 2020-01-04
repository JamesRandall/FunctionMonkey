using System.Linq;
using System.Threading.Tasks;
using FunctionMonkey.Commanding.Abstractions.Validation;
using FunctionMonkey.Testing.Tests.Helpers;
using FunctionMonkey.Tests.Integration.Common.Commands;
using FunctionMonkey.Tests.Integration.Common.Commands.HttpResponseShaping;
using FunctionMonkey.Tests.Integration.Common.Commands.Model;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace FunctionMonkey.Testing.Tests
{
    public class HttpTestsWithNoResponseHandlerShould : AbstractAcceptanceTest
    {
        [Fact]
        public async Task ReturnOkResultForCommandWithResultWhenNoValidator()
        {
            HttpResponse httpResponse = await ExecuteHttpAsync(new HttpCommandWithResultAndNoValidation
            {
                Value = 5
            });

            Assert.Equal(200, httpResponse.StatusCode);
            SimpleResponse result = httpResponse.GetJson<SimpleResponse>();
            Assert.Equal(1, result.Value);
            Assert.Equal("success", result.Message);
        }

        [Fact]
        public async Task ReturnOkResultForCommandWithResultWhenValidatorPasses()
        {
            HttpResponse httpResponse = await ExecuteHttpAsync(new HttpCommandWithResultAndValidatorThatPasses());

            Assert.Equal(200, httpResponse.StatusCode);
            SimpleResponse result = httpResponse.GetJson<SimpleResponse>();
            Assert.Equal(1, result.Value);
            Assert.Equal("success", result.Message);
        }

        [Fact]
        public async Task ReturnOkResultForCommandWithResultWhenValidatorFails()
        {
            HttpResponse httpResponse = await ExecuteHttpAsync(new HttpCommandWithResultAndValidatorThatFails());

            Assert.Equal(400, httpResponse.StatusCode);
            ValidationResult validationResult = httpResponse.GetJson<ValidationResult>();
            Assert.False(validationResult.IsValid);
            Assert.NotNull(validationResult.Errors.SingleOrDefault(x => x.Property == "Value"));
            Assert.NotNull(validationResult.Errors.SingleOrDefault(x => x.ErrorCode == "NotEqualValidator"));
        }

        [Fact]
        public async Task ReturnOkResultForCommandWithNoResultWhenNoValidator()
        {
            HttpResponse httpResponse = await ExecuteHttpAsync(new HttpCommandWithNoResultAndNoValidation
            {
                Value = 5
            });

            Assert.Equal(200, httpResponse.StatusCode);
            Assert.Equal(0, httpResponse.ContentLength);
        }

        [Fact]
        public async Task ReturnOkResultForCommandWithNoResultWhenValidatorPasses()
        {
            HttpResponse httpResponse = await ExecuteHttpAsync(new HttpCommandWithNoResultAndValidatorThatPasses());

            Assert.Equal(200, httpResponse.StatusCode);
            Assert.Equal(0, httpResponse.ContentLength);
        }

        [Fact]
        public async Task ReturnOkResultForCommandWithNoResultWhenValidatorFails()
        {
            HttpResponse httpResponse = await ExecuteHttpAsync(new HttpCommandWithNoResultAndValidatorThatFails());

            Assert.Equal(400, httpResponse.StatusCode);
            ValidationResult validationResult = httpResponse.GetJson<ValidationResult>();
            Assert.False(validationResult.IsValid);
            Assert.NotNull(validationResult.Errors.SingleOrDefault(x => x.Property == "Value"));
            Assert.NotNull(validationResult.Errors.SingleOrDefault(x => x.ErrorCode == "NotEqualValidator"));
        }

        [Fact]
        public async Task ReturnsOkForCommandWithNoResultAndLogger()
        {
            HttpResponse httpResponse = await ExecuteHttpAsync(new HttpGetWithLoggerCommand());
            Assert.Equal(200, httpResponse.StatusCode);
            Assert.Equal(0, httpResponse.ContentLength);
        }
    }
}
