using System.Net.Http;
using FmAspNetCore.Commands;
using FmAspNetCore.Services;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Compiler.Core;
using Microsoft.Extensions.DependencyInjection;

namespace FmAspNetCore
{
    public class FunctionAppConfiguration : IFunctionAppConfiguration
    {
        public void Build(IFunctionHostBuilder builder)
        {
            builder
                .CompilerOptions(options => options
                    .HttpTarget(CompileTargetEnum.AspNetCore) // the magic!
                    .OutputSourceTo(@"/Users/jamesrandall/code/authoredSource")
                )
                .Setup((sc, r) =>
                {
                    sc.AddSingleton<IRepository, Repository>();
                    r.Discover<FunctionAppConfiguration>();
                })
                .Functions(functions => functions
                    .HttpRoute("todo", route => route
                        .HttpFunction<CreateTodoItemCommand>(HttpMethod.Post)
                        .HttpFunction<GetAllTodoItemsQuery>(HttpMethod.Get)
                        .HttpFunction<MarkCompleteCommand>(HttpMethod.Put)
                    )
                );
        }
    }
}

