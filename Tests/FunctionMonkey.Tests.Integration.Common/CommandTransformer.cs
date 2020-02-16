using FunctionMonkey.Abstractions;
using FunctionMonkey.Tests.Integration.Common.Commands;

namespace FunctionMonkey.Tests.Integration.Common
{
    public class CommandTransformer : ICommandTransformer
    {
        public TCommand Transform<TCommand>(TCommand input)
        {
            if (input is HttpGetCommandWithTransformer command)
            {
                command.Value++;
            }

            return input;
        }
    }
}