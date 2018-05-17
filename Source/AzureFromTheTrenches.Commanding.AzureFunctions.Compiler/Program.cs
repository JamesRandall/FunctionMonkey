using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using AzureFromTheTrenches.Commanding.AzureFunctions.Compiler.Implementation;

namespace AzureFromTheTrenches.Commanding.AzureFunctions.Compiler
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                throw new ArgumentException("Must specify at least <ASSMEBLY_FILE> and <OUTPUT_DIR>");
            }

            string inputAssemblyFile = args[0];
            string outputFunctionDirectory = args[1];

            
            Assembly assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(inputAssemblyFile);
            string outputBinaryDirectory = Path.GetDirectoryName(assembly.Location);
            
            // Not sure why the AssemblyLoadContext doesn't deal with the below. I thought it did. Clearly not.
            // TODO: Have a chat with someone who knows a bit more about this.
            AssemblyLoadContext.Default.Resolving += (context, name) =>
            {
                string path = $"{outputBinaryDirectory}\\{name.Name}.dll";
                if (File.Exists(path))
                {
                    Assembly referencedAssembly = context.LoadFromAssemblyPath(path);
                    return referencedAssembly;
                }                
                return null;
            };

            FunctionCompiler compiler = new FunctionCompiler(assembly, outputBinaryDirectory, outputFunctionDirectory);
            compiler.Compile().Wait();
        }
    }
}
