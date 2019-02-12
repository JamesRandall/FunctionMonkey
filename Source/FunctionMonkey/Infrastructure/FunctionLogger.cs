using System;
using Microsoft.Extensions.Logging;

namespace FunctionMonkey.Infrastructure
{
    class FunctionLogger : ILogger
    {
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            Runtime.FunctionProvidedLogger.Value.Log(logLevel, eventId, state, exception, formatter);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return Runtime.FunctionProvidedLogger.Value.IsEnabled(logLevel);
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return Runtime.FunctionProvidedLogger.Value.BeginScope(state);
        }
    }
}
