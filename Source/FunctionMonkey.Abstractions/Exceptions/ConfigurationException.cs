using System;

namespace FunctionMonkey
{
    /// <summary>
    /// An exception thrown when there are unexpected configuration issues
    /// </summary>
    public class ConfigurationException : Exception
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">The error message</param>
        public ConfigurationException(string message) : base(message)
        {
        }
    }
}
