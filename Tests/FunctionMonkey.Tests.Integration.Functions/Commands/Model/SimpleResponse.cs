using System.Threading.Tasks;

namespace FunctionMonkey.Tests.Integration.Functions.Commands.Model
{
    public class SimpleResponse
    {
        public int Value { get; set; }

        public string Message { get; set; }

        public static Task<SimpleResponse> Success()
        {
            return Task.FromResult(new SimpleResponse
            {
                Message = "success",
                Value = 1
            });
        }
    }
}
