using System;
using System.Collections.Generic;
using System.Text;

namespace AzureFromTheTrenches.Commanding.AzureFunctions.Compiler
{
    public class CompilerException : Exception
    {
        public CompilerException(string message) : base(message)
        {
        }
    }
}
