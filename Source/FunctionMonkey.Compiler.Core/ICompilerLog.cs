namespace FunctionMonkey.Compiler
{
    public interface ICompilerLog
    {
        void Error(string message, params object[] args);
        
        void Warning(string message, params object[] args);
        
        void Message(string message, params object[] args);
    }
}