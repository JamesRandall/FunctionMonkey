using System;

namespace FunctionMonkey
{
    public class DeserializationException : Exception
    {
        public string Path { get; }
        
        public int LineNumber { get; }
        
        public int LinePosition { get; }
        
        public DeserializationException(string path, int lineNumber, int linePosition)
        {
            Path = path;
            LineNumber = lineNumber;
            LinePosition = linePosition;
        }

        public DeserializationException(string message) : base(message)
        {
            Path = null;
            LineNumber = -1;
            LinePosition = -1;
        }
    }
}