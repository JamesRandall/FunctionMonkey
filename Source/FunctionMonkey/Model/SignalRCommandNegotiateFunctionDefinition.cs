using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionMonkey.Model
{
    public class SignalRCommandNegotiateFunctionDefinition : HttpFunctionDefinition
    {
        public SignalRCommandNegotiateFunctionDefinition(Type commandType) : base(commandType)
        {

        }

        public string ConnectionStringSettingName { get; set; }
    }
}
