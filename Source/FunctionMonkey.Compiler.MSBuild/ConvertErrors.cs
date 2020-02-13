using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Build.Utilities;

namespace FunctionMonkey.Compiler.MSBuild
{
    public class ConvertErrors : Task
    {
        public string InputPath { get; set; }
        
        public override bool Execute()
        {
            if (string.IsNullOrWhiteSpace(InputPath))
            {
                Log.LogWarning("No input path specified");
                return true;
            }

            string file = Path.Combine(InputPath, "__fm__errors.json");
            if (!File.Exists(file))
            {
                Log.LogWarning("Missing error file");
            }
            string json = File.ReadAllText(file);
            MSBuildErrorItem[] items = SimpleJson.SimpleJson.DeserializeObject<MSBuildErrorItem[]>(json);
            bool hasError = false;
            foreach(MSBuildErrorItem item in items)
            {
                if (item.Severity == MSBuildErrorItem.SeverityLevel.Error)
                {
                    Log.LogError(item.Message);
                    hasError = true;
                }
                else if (item.Severity == MSBuildErrorItem.SeverityLevel.Warning)
                {
                    Log.LogWarning(item.Message);
                }
                else
                {
                    Log.LogMessage(item.Message);
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