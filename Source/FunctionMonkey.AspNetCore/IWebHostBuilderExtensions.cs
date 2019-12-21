using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;

namespace FunctionMonkey.AspNetCore
{
    // ReSharper disable once InconsistentNaming
    public static class IWebHostBuilderExtensions
    {
        public static IWebHostBuilder UseFunctionMonkey(this IWebHostBuilder builder)
        {
            Assembly callingAssembly = Assembly.GetCallingAssembly();
            string path = Path.GetDirectoryName(callingAssembly.Location);
            string filename = $"{callingAssembly.GetName().Name}.Functions.dll";
            string assemblyFilename = Path.Combine(path, filename);
            byte[] assemblyBytes = File.ReadAllBytes(assemblyFilename);
            Assembly assembly = Assembly.Load(assemblyBytes);
            AppDomain.CurrentDomain.AssemblyResolve += (sender, eventArgs) =>
            {
                string requestedShortName = eventArgs.Name.Split(',')[0]?.Trim();
                string compiledAssemblyName = assembly.GetName().Name;
                
                if (requestedShortName == compiledAssemblyName)
                {
                    return assembly;
                }
                return null;
            };
            
            Type startupType = assembly.GetTypes().Single(x => x.Name == "Startup");
            builder.UseStartup(startupType);
            return builder;
        }
    }
}