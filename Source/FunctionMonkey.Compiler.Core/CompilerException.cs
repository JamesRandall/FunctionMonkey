using System;

namespace FunctionMonkey.Compiler
{
    public class CompilerException : Exception
    {
        public CompilerException(string message) : base(message)
        {
        }
    }
}
