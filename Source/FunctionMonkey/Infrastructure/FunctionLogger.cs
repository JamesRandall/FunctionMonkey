using System;
using Microsoft.Extensions.Logging;

namespace FunctionMonkey.Infrastructure
{
    class FunctionLogger : ILogger
    {
        private readonly RuntimeInstance _runtimeInstance;

        public FunctionLogger(RuntimeInstance runtimeInstance)
        {
            _runtimeInstance = runtimeInstance;
        }
        
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            _runtimeInstance.FunctionProvidedLogger.Value.Log(logLevel, eventId, state, exception, formatter);
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
