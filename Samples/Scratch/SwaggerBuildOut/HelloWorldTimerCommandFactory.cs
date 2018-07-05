using FunctionMonkey.Abstractions;
using SwaggerBuildOut.Commands;

namespace SwaggerBuildOut
{
    internal class HelloWorldTimerCommandFactory : ITimerCommandFactory<HelloWorldCommand>
    {
        public HelloWorldCommand Create(string cronExpression)
        {
            return new HelloWorldCommand {Name = cronExpression};
        }
    }
}
