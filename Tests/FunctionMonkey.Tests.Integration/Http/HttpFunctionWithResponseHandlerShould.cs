using System.Threading.Tasks;
using Flurl;
using Flurl.Http;

using Xunit;

namespace FunctionMonkey.Tests.Integration.Http
{
    public class HttpFunctionWithResponseHandlerShould : AbstractHttpFunctionTest
    {
        [Fact]
        public async Task ReturnTransformedResponseWhenCommandHasNoResultAndNoValidation()
        {
            string response = await Settings.Host
                .AppendPathSegment("/responseHandler/noResult/noValidation")
                .GetStringAsync();

            Assert.Equal("CreateResponse<TCommand>", response);
        }

        [Fact]
        public async Task ReturnTransformedResponseWhenCommandHasNoResultAndPassesValidation()
        {
            string response = await Settings.Host
                .AppendPathSegment("/responseHandler/noResult/validationPasses")
                .GetStringAsync();

            Assert.Equal("CreateResponse<TCommand>", response);
        }

        [Fact]
        public async Task ReturnTransformedResponseWhenCommandHasNoResultAndFailsValidation()
        {
            string response = await Settings.Host
                .AppendPathSegment("/responseHandler/noResult/validationFails")
                .GetStringAsync();

            Assert.Equal("CreateValidationFailureResponse<TCommand>", response);
        }

        [Fact]
        public async Task ReturnTransformedResponseWhenCommandHasResultAndNoValidation()
        {
            string response = await Settings.Host
                .AppendPathSegment("/responseHandler/result/noValidation")
                .GetStringAsync();

            Assert.Equal("CreateResponse<TCommand,TResult>", response);
        }

        [Fact]
        public async Task ReturnTransformedResponseWhenCommandHasResultValidationPasses()
        {
            string response = await Settings.Host
                .AppendPathSegment("/responseHandler/result/validationPasses")
                .GetStringAsync();

            Assert.Equal("CreateResponse<TCommand,TResult>", response);
        }

        [Fact]
        public async Task ReturnTransformedResponseWhenCommandHasResultValidationFails()
        {
            string response = await Settings.Host
                .AppendPathSegment("/responseHandler/result/validationFails")
                .GetStringAsync();

            Assert.Equal("CreateValidationFailureResponse<TCommand>", response);                       
        }
    }
}
