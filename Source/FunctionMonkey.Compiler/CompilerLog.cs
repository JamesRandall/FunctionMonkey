using System.Collections.Generic;
using System.Linq;
using System.Text;
using FunctionMonkey.Compiler.Core;
using Newtonsoft.Json;

namespace FunctionMonkey.Compiler
{
    internal class CompilerLog : ICompilerLog
    {
        private class Item
        {
            public enum SeverityEnum
            {
                Error,
                Warning,
                Message
            }

            public SeverityEnum Severity { get; set; }
            
            public string Message { get; set; }
        }
        
        private readonly List<Item> _items = new List<Item>();
        
        public void Error(string message, params object[] args)
        {
            _items.Add(new Item()
            {
                Severity = Item.SeverityEnum.Error,
                Message = message
            });
        }

        public void Warning(string message, params object[] args)
        {
            _items.Add(new Item()
            {
                Severity = Item.SeverityEnum.Warning,
                Message = message
            });
        }

        public void Message(string message, params object[] args)
        {
            _items.Add(new Item()
            {
                Severity = Item.SeverityEnum.Message,
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
            foreach (Item item in _items)
            {
                sb.AppendLine($"{item.Severity.ToString().ToUpper()}: {item.Message}");
            }

            return sb.ToString();
        }

        public bool HasItems => _items.Any();
    }
}