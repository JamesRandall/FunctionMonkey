using FunctionMonkey.Abstractions;
using FunctionMonkey.Tests.Integration.Common.Commands.TestInfrastructure;

namespace FunctionMonkey.Tests.Integration.Common
{
    public class OptionalValueCommandConverter : IOutputBindingConverter
    {
        public object Convert(object input)
        {
            if (input is IOptionalValueCommand valueCommand)
            {
                valueCommand.Value = valueCommand.Value + 1;
            }

            return input;
        }
    }
}