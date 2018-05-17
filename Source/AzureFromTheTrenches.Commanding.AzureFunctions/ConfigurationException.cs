using System;

namespace AzureFromTheTrenches.Commanding.AzureFunctions
{
    public class ConfigurationException : Exception
    {
        public ConfigurationException(string message) : base(message)
        {
        }
    }
}
