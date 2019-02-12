using System;
using System.Collections.Generic;
using System.Text;

namespace SwaggerBuildOut.Services
{
    class MessageProvider : IMessageProvider
    {
        public string GetMessage()
        {
            return "hello";
        }
    }
}
