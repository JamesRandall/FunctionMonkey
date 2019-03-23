using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionMonkey.Model
{
    public class SignalRNegotiateFunctionDefinition : HttpFunctionDefinition
    {
        public SignalRNegotiateFunctionDefinition(Type commandType) : base(commandType)
        {

        }

        public string ConnectionStringSettingName { get; set; }
    }
}
