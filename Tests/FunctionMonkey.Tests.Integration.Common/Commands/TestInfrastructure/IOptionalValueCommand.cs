namespace FunctionMonkey.Tests.Integration.Common.Commands.TestInfrastructure
{
    public interface IOptionalValueCommand
    {
        public int? Value { get; set; }
    }
}