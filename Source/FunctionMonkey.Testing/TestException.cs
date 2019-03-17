using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionMonkey.Testing
{
    public class TestException : Exception
    {
        public TestException(string message) : base(message)
        {
        }
    }
}
