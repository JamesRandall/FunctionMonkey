using FunctionMonkey.Commanding.Abstractions;

namespace FunctionMonkey.Tests.Integration.Functions.Commands.Model
{
    public class NamedBlobOutputResponse : IBlobOutputCommandResult<BlobOutputResponse>
    {
        public string Name { get; set; }
        public BlobOutputResponse Value { get; set; }
    }
}
