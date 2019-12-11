using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using FunctionMonkey.Compiler.Implementation;
using Microsoft.Build.Utilities;

namespace FunctionMonkey.Compiler.MSBuild
{
    public class CompileFunctions : Task
    {
        public string InputAssemblyPath { get; set; }
        
        public override bool Execute()
        {
            CompileTargetEnum target = CompileTargetEnum.NETCore21;
            
            // TODO: convert the input to an absolute path if necessary
            Assembly assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(InputAssemblyPath);
            string outputBinaryDirectory = Path.GetDirectoryName(assembly.Location);
            
            // Not sure why the AssemblyLoadContext doesn't deal with the below. I thought it did. Clearly not.
            // TODO: Have a chat with someone who knows a bit more about this.
            AssemblyLoadContext.Default.Resolving += (context, name) =>
            {
                string path = Path.Combine(outputBinaryDirectory, $"{name.Name}.dll");
                //string path = $"{outputBinaryDirectory}\\{name.Name}.dll";
                if (File.Exists(path))
                {
                    Assembly referencedAssembly = context.LoadFromAssemblyPath(path);
                    return referencedAssembly;
                }                
                return null;
            };

            CompilerLog log = new CompilerLog(Log);
            FunctionCompiler compiler = new FunctionCompiler(assembly, outputBinaryDirectory, target, log);
            compiler.Compile();

            return !Log.HasLoggedErrors;
        }
    }
}