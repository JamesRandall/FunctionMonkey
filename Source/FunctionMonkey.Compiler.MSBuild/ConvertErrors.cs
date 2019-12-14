using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Build.Utilities;

namespace FunctionMonkey.Compiler.MSBuild
{
    public class ConvertErrors : Task
    {
        public string InputAssemblyPath { get; set; }
        
        public override bool Execute()
        {
            Log.LogWarning("Hitting clean up compiler");
            return false;
        }
    }
}