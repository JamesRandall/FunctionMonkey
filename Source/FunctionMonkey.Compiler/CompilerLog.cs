using System.Collections.Generic;
using System.Linq;
using System.Text;
using FunctionMonkey.Compiler.Core;
using FunctionMonkey.Compiler.MSBuild;
using Newtonsoft.Json;

namespace FunctionMonkey.Compiler
{
    internal class CompilerLog : ICompilerLog
    {
        private readonly List<MSBuildErrorItem> _items = new List<MSBuildErrorItem>();
        
        public void Error(string message, params object[] args)
        {
            _items.Add(new MSBuildErrorItem()
            {
                Severity = MSBuildErrorItem.SeverityLevel.Error,
                Message = message
            });
        }

        public void Warning(string message, params object[] args)
        {
            _items.Add(new MSBuildErrorItem()
            {
                Severity = MSBuildErrorItem.SeverityLevel.Warning,
                Message = message
            });
        }

        public void Message(string message, params object[] args)
        {
            _items.Add(new MSBuildErrorItem()
            {
                Severity = MSBuildErrorItem.SeverityLevel.Message,
                Message = message
            });
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(_items);
        }

        public string ToConsole()
        {
            StringBuilder sb = new StringBuilder();
            foreach (MSBuildErrorItem item in _items)
            {
                sb.AppendLine($"{item.Severity.ToString().ToUpper()}: {item.Message}");
            }

            return sb.ToString();
        }

        public bool HasItems => _items.Any();
    }
}