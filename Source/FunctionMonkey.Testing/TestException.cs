using System;

namespace FunctionMonkey.Testing
{
    public class TestException : Exception
    {
        public TestException(string message) : base(message)
        {
        }
    }
}
