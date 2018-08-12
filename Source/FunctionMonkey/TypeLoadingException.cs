using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionMonkey
{
    public class TypeLoadingException : Exception
    {
        public TypeLoadingException(string message) : base(message)
        {
        }
    }
}
