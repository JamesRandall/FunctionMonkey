using System.Collections.Generic;
using System.Threading.Tasks;

namespace FunctionMonkey.Tests.Integration.Common.Commands.Model
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

        public static Task<IReadOnlyCollection<SimpleResponse>> SuccessCollection()
        {
            IReadOnlyCollection<SimpleResponse> collection = new SimpleResponse[]
            {
                new SimpleResponse
                {
                    Message = "success1",
                    Value = 1
                },
                new SimpleResponse
                {
                    Message = "success2",
                    Value = 2
                }
            };

            return Task.FromResult(collection);
        }
    }
}
