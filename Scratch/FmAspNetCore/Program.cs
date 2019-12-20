using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using FmAspNetCore.Commands;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Compiler.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FmAspNetCore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
            {
                Console.WriteLine(eventArgs.ExceptionObject.ToString());
            };
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            string path = System.IO.Path.GetDirectoryName(typeof(Program).Assembly.Location);
            string assemblyFilename = Path.Combine(path, "FmAspNetCore.Functions.dll");
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
            //Assembly assembly = AppDomain.CurrentDomain.Load(assemblyBytes);
            //Assembly assembly = Assembly.LoadFile(assemblyFilename);
            Type startupType = assembly.GetTypes().Single(x => x.Name == "Startup");
            object created = Activator.CreateInstance(startupType);
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup(startupType); });
        }
    }

    public class FunctionAppConfiguration : IFunctionAppConfiguration
    {
        public void Build(IFunctionHostBuilder builder)
        {
            builder
                .CompilerOptions(options => options
                    .HttpTarget(CompileTargetEnum.AspNetCore)
                )
                .Setup((sc, r) => { r.Discover<FunctionAppConfiguration>(); })
                .Functions(functions => functions
                    .HttpRoute("todo", route => route
                        .HttpFunction<CreateTodoItemCommand>(HttpMethod.Post)
                    )
                );
        }
    }
}