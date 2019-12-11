using Microsoft.Build.Utilities;

namespace FunctionMonkey.Compiler.MSBuild
{
    internal class CompilerLog : ICompilerLog
    {
        private readonly TaskLoggingHelper _logger;

        public CompilerLog(TaskLoggingHelper logger)
        {
            _logger = logger;
        }
        
        public void Error(string message, params object[] args)
        {
            _logger.LogError(message, args);
        }

        public void Warning(string message, params object[] args)
        {
            _logger.LogWarning(message, args);
        }

        public void Message(string message, params object[] args)
        {
            _logger.LogMessage(message, args);
        }
    }
}