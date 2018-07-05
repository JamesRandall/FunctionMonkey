using System;

namespace FunctionMonkey.Model
{
    public class TimerFunctionDefinition : AbstractFunctionDefinition
    {
        public TimerFunctionDefinition(Type commandType) : base("Timer", commandType)
        {
        }

        public string CronExpression { get; set; }

        public Type TimerCommandFactoryType { get; set; }

        public string TimerCommandFactoryTypeName { get; set; }
    }
}
