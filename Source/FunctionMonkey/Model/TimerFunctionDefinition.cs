using System;
using FunctionMonkey.Abstractions.Builders.Model;

namespace FunctionMonkey.Model
{
    public class TimerFunctionDefinition : AbstractFunctionDefinition
    {
        public TimerFunctionDefinition(Type commandType) : base("Timer", commandType)
        {
        }
        
        public TimerFunctionDefinition(Type commandType, Type resultType) : base("Timer", commandType, resultType)
        {
        }

        public string CronExpression { get; set; }

        public Type TimerCommandFactoryType { get; set; }

        public string TimerCommandFactoryTypeName { get; set; }
    }
}
