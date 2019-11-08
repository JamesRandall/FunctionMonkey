using Microsoft.Extensions.Logging;

namespace FunctionMonkey.Infrastructure
{
    class FunctionLoggerFactory : ILoggerFactory
    {
        private readonly RuntimeInstance _runtimeInstance;

        public FunctionLoggerFactory(RuntimeInstance runtimeInstance)
        {
            _runtimeInstance = runtimeInstance;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new FunctionLogger(_runtimeInstance);
        }

        public void AddProvider(ILoggerProvider provider)
        {
        }

        public void Dispose()
        {
        }
    }
}
