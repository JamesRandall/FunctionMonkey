using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace FunctionMonkey.Compiler
{
    class Program
    {
        enum LogOutputType
        {
            Console,
            Json
        }
        
        static void Main(string[] args)
        {
            CompilerLog compilerLog = new CompilerLog();
            compilerLog.Message("Compiler starting");
            LogOutputType outputType = args.Any(x => x.ToLower() == "--jsonoutput")
                ? LogOutputType.Json
                : LogOutputType.Console;
            
            string outputBinaryDirectory = String.Empty;
            
            if (args.Length == 0)
            {
                compilerLog.Error("Must specify the assembly file to build the functions from");
            }
            else
            {
                try
                {
                    string inputAssemblyFile = args[0];
                    compilerLog.Message($"Loading assembly {inputAssemblyFile}");
                    // TODO: convert the input to an absolute path if necessary
                    Assembly assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(inputAssemblyFile);
                    outputBinaryDirectory = Path.GetDirectoryName(assembly.Location);
            
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

                    Core.Compiler compiler = new Core.Compiler(assembly, outputBinaryDirectory, compilerLog);
                    compiler.Compile();
                }
                catch (Exception e)
                {
                    compilerLog.Error($"Unexpected error: {e.Message}\n{e.StackTrace}");
                }
            }
            compilerLog.Message("Compilation complete");

            if (compilerLog.HasItems)
            {
                if (string.IsNullOrWhiteSpace(outputBinaryDirectory) && outputType == LogOutputType.Json)
                {
                    compilerLog.Warning("Cannot write errors to output file as no output directory can be found, likely a missing assembly");
                    outputType = LogOutputType.Console;
                }

                if (outputType == LogOutputType.Console)
                {
                    System.Console.WriteLine(compilerLog.ToConsole());
                }
                else
                {
                    string outputPath = Path.Combine(outputBinaryDirectory, "__fm__errors.json");
                    File.WriteAllText(outputPath, compilerLog.ToJson());
                }
            }
        }
    }
}
