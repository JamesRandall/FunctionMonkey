using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Build.Utilities;
using Newtonsoft.Json;

namespace FunctionMonkey.Compiler.MSBuild
{
    public class ConvertErrors : Task
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
        
        public string InputAssemblyPath { get; set; }
        
        public override bool Execute()
        {
            string path = Path.GetDirectoryName(InputAssemblyPath);
            if (string.IsNullOrWhiteSpace(path))
            {
                Log.LogWarning("No input path specified");
                return true;
            }

            string file = Path.Combine(path, "__fm__errors.json");
            if (!File.Exists(file))
            {
                Log.LogWarning("FUNCTION MONKEY: Missing error file");
            }
            string json = File.ReadAllText(file);
            List<Item> items = JsonConvert.DeserializeObject<List<Item>>(json);
            bool hasError = false;
            foreach(Item item in items)
            {
                if (item.Severity == Item.SeverityEnum.Error)
                {
                    Log.LogError($"FUNCTION MONKEY: {item.Message}");
                    hasError = true;
                }
                else if (item.Severity == Item.SeverityEnum.Warning)
                {
                    Log.LogWarning($"FUNCTION MONKEY: {item.Message}");
                }
                else
                {
                    Log.LogMessage($"FUNCTION MONKEY: {item.Message}");
                }
            }

            try
            {
                File.Delete(file);
            }
            catch (Exception)
            {
                Log.LogWarning("FUNCTION MONKEY: Unable to remove error file");
            }
            
            return !hasError;
        }
    }
}