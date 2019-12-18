using System;

namespace FunctionMonkey.Compiler.Core
{
    public class CompilerException : Exception
    {
        public CompilerException(string message) : base(message)
        {
        }
    }
}
