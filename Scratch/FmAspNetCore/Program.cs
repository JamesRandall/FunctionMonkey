using System;
using System.Net.Http;
using FmAspNetCore.Commands;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.AspNetCore;
using FunctionMonkey.Compiler.Core;
using Microsoft.Extensions.Hosting;

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
        
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseFunctionMonkey(); });
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