using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionMonkey.Abstractions.SignalR
{
    public class SignalRMessage
    {
        public string UserId { get; set; }

        public string GroupName { get; set; }

        public string Target { get; set; }

        public object[] Arguments { get; set; }
    }
}
