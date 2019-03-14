using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using FunctionMonkey.Tests.Integration.Http.Helpers;
using Xunit;

namespace FunctionMonkey.Tests.Integration.Http
{
    public class HttpFunctionWithNoResponseHandlerShould : AbstractHttpFunctionTest
    {
        [Fact]
        public async Task ReturnOkWhenCommandHasNoResultAndNoValidation()
        {
            HttpResponseMessage response = await Settings.Host
                .AppendPathSegment("/noResponseHandler/noResult/noValidation")
                .GetAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task ReturnOkWhenCommandHasNoResultAndPassesValidation()
        {
            HttpResponseMessage response = await Settings.Host
                .AppendPathSegment("/noResponseHandler/noResult/validationPasses")
                .GetAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task ReturnBadRequestWhenCommandHasNoResultAndFailsValidation()
        {
            HttpResponseMessage response = await Settings.Host
                .AppendPathSegment("/noResponseHandler/noResult/validationFails")
                .AllowHttpStatus(HttpStatusCode.BadRequest)
                .GetAsync();

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task ReturnOkAndValidPayloadWhenCommandHasResultAndNoValidation()
        {
            SimpleResponse response = await Settings.Host
                .AppendPathSegment("/noResponseHandler/result/noValidation")
                .GetJsonAsync<SimpleResponse>();

            response.ValidateResponse();
        }

        [Fact]
        public async Task ReturnOkAndValidPayloadWhenCommandHasResultValidationPasses()
        {
            SimpleResponse response = await Settings.Host
                .AppendPathSegment("/noResponseHandler/result/validationPasses")
                .GetJsonAsync<SimpleResponse>();

            response.ValidateResponse();
        }

        [Fact]
        public async Task ReturnOkAndValidPayloadWhenCommandHasResultValidationFails()
        {
            try
            {
                await Settings.Host
                    .AppendPathSegment("/noResponseHandler/result/validationFails")
                    .GetJsonAsync<SimpleResponse>();
                Assert.True(false); // force a failure as we are expecting an exception generating bad response
            }
            catch (FlurlHttpException fex)
            {
                Assert.Equal(HttpStatusCode.BadRequest, fex.Call.HttpStatus);
                ValidationResult result = await fex.GetResponseJsonAsync<ValidationResult>();
                result.ValidateResponse();
            }            
        }
    }
}
