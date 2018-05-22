using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using FunctionMonkey.Compiler.Implementation;

namespace FunctionMonkey.Compiler
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                throw new ArgumentException("Must specify the assembly file to build the functions from");
            }

            string inputAssemblyFile = args[0];
            bool outputProxiesJson = true;
            if (args.Length > 1)
            {
                outputProxiesJson = bool.Parse(args[1]);
            }
            
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

            FunctionCompiler compiler = new FunctionCompiler(assembly, outputBinaryDirectory, outputProxiesJson);
            compiler.Compile().Wait();
        }
    }
}
