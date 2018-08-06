using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionMonkey.Abstractions.Contexts
{
    public class ExecutionContext
    {
        public string FunctionAppDirectory { get; set; }

        public string FunctionDirectory { get; set; }

        public string FunctionName { get; set; }

        public Guid InvocationId { get; set; }
    }
}
