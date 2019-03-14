using Xunit;

namespace FunctionMonkey.Tests.Integration.Http.Helpers
{
    public class SimpleResponse
    {
        public int Value { get; set; }

        public string Message { get; set; }

        public void ValidateResponse()
        {
            Assert.Equal("success", Message);
            Assert.Equal(1, Value);
        }
    }
}
