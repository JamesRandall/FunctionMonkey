using System.Net.Http;
using FmAspNetCore.Commands;
using FmAspNetCore.Services;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Compiler.Core;
using FunctionMonkey.TokenValidator;
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
                    //.OutputSourceTo(@"/Users/jamesrandall/code/authoredSource")
                )
                .Authorization(auth => auth
                    .AuthorizationDefault(AuthorizationTypeEnum.TokenValidation)
                    .AddOpenIdConnectTokenValidator("https://accidentalfish.eu.auth0.com/.well-known/openid-configuration")
                )
                .OpenApiEndpoint(openApi => openApi
                    .Title("My API")
                    .Version("0.0.0")
                    .UserInterface()
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
                    .HttpRoute("loaderio-fa4864b7cff0c92b67ffdc6c1b85d9a5", route => route
                        .HttpFunction<LoaderIoQuery>(HttpMethod.Get).Options(options => options.ResponseHandler<StringContentResponseHandler>())
                    )
                );
        }
    }
}

