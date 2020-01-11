using System.Net.Http;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.MediatR;
using MediatR;
using MediatRSample.Services;
using Microsoft.Extensions.DependencyInjection;
using ToDo.Application.Commands;
using ToDo.Application.Services;

namespace MediatRSample
{
    public class FunctionAppConfiguration : IFunctionAppConfiguration
    {
        public void Build(IFunctionHostBuilder builder)
        {
            builder
                .Setup(sc => sc
                    .AddMediatR(typeof(FunctionAppConfiguration).Assembly)
                    .AddSingleton<IRepository, Repository>()
                )
                .UseMediatR()
                .Functions(functions => functions
                    .HttpRoute("todo", route => route
                        .HttpFunction<GetAllToDoItemsQuery>(HttpMethod.Get)
                        .HttpFunction<MarkToDoItemCompleteCommand>("complete", HttpMethod.Put)
                        .HttpFunction<CreateToDoItemCommand>(HttpMethod.Post)
                    )
                );
        }
    }
}