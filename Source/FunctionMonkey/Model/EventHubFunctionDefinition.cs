using System;

namespace FunctionMonkey.Model
{
    public class EventHubFunctionDefinition : AbstractFunctionDefinition
    {
        public EventHubFunctionDefinition(Type commandType) : base("EhFn",commandType)
        {
        }

        public string ConnectionStringName { get; set; }

        public string EventHubName { get; set; }
    }
}
