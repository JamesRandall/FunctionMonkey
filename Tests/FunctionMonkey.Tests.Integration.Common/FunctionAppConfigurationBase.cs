using System.Collections.Generic;
using System.Net.Http;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Abstractions.Builders.Model;
using FunctionMonkey.FluentValidation;
using FunctionMonkey.Tests.Integration.Common.Commands;
using FunctionMonkey.Tests.Integration.Common.Commands.HttpResponseShaping;
using FunctionMonkey.Tests.Integration.Common.Commands.TestInfrastructure;
using FunctionMonkey.Tests.Integration.Common.Services;
using FunctionMonkey.Tests.Integration.Common.Validators;
using FunctionMonkey.Tests.Integration.Functions;
using Microsoft.Extensions.DependencyInjection;

namespace FunctionMonkey.Tests.Integration.Common
{
    public class FunctionAppConfigurationBase : IFunctionAppConfiguration
    {
        
        
        protected IFunctionBuilder CreateCommonFunctions(IFunctionBuilder builder)
        {
            return builder
                // this is not really part of the test suite - but it needs to work - it sets up tables, containers, queues etc.
                // essentially pre-reqs for tracking things in the test suite
                .HttpRoute("setup", route => route
                    .HttpFunction<SetupTestResourcesCommand>(HttpMethod.Put)
                )
                .HttpRoute("claims", route => route
                    .HttpFunction<HttpStringClaimCommand>("/string", AuthorizationTypeEnum.TokenValidation,
                        HttpMethod.Get)
                    .HttpFunction<HttpIntClaimCommand>("/int", AuthorizationTypeEnum.TokenValidation, HttpMethod.Get)
                )
                .HttpRoute("verbs", route => route
                    .HttpFunction<HttpGetCommand>("/{value}", HttpMethod.Get)
                    .HttpFunction<HttpPostCommand>(HttpMethod.Post)
                    .HttpFunction<HttpPostWithBytesCommand>("/bytes", HttpMethod.Post)
                    .HttpFunction<HttpPutCommand>(HttpMethod.Put)
                    .HttpFunction<HttpDeleteCommand>("/{value}", HttpMethod.Delete)
                    .HttpFunction<HttpPatchCommand>(new HttpMethod("PATCH"))
                )
                .HttpRoute("transformer", route => route
                    .HttpFunction<HttpGetCommandWithTransformer>("/{value}", HttpMethod.Get)
                    .Options(options => options.CommandTransformer<CommandTransformer>())
                )
                .HttpRoute("securityProperty", route => route
                    .HttpFunction<HttpPostCommandWithSecurityProperty>(HttpMethod.Post)
                    .HttpFunction<HttpGetCommandWithSecurityProperty>(HttpMethod.Get)
                )
                .HttpRoute("withLogger", route => route
                    .HttpFunction<HttpGetWithLoggerCommand>(HttpMethod.Get)
                )
                .HttpRoute("queryParameters", route => route
                    .HttpFunction<HttpGetQueryParamCommand>(HttpMethod.Get)
                    .HttpFunction<HttpGetGuidQueryParameterCommand>("/guidQueryParam", HttpMethod.Get)
                    .HttpFunction<HttpArrayQueryParamCommand>("/array", HttpMethod.Get)
                    .HttpFunction<HttpIReadOnlyCollectionQueryParamCommand>("/readonlyCollection", HttpMethod.Get)
                    .HttpFunction<HttpListQueryParamCommand>("/list", HttpMethod.Get)
                    .HttpFunction<HttpListQueryStringParamCommand>("/stringList", HttpMethod.Get)
                    .HttpFunction<HttpIEnumerableQueryParamCommand>("/enumerable", HttpMethod.Get)
                )
                .HttpRoute("routeParameters", route => route
                    .HttpFunction<HttpGetRouteParameterCommand>(
                        "/{message}/{value:int}/{optionalValue?}/{optionalMessage?}")
                    .HttpFunction<HttpGetGuidRouteParameterCommand>("/guids/{requiredGuid}/{optionalGuid?}")
                )
                .HttpRoute("noResponseHandler", route => route
                    // These are the functions for testing the HTTP route cases outlined above
                    .HttpFunction<HttpCommandWithNoResultAndNoValidation>("/noResult/noValidation")
                    .HttpFunction<HttpCommandWithNoResultAndValidatorThatFails>("/noResult/validationFails")
                    .HttpFunction<HttpCommandWithNoResultAndValidatorThatPasses>("/noResult/validationPasses")
                    .HttpFunction<HttpCommandWithResultAndNoValidation>("/result/noValidation")
                    .HttpFunction<HttpCommandWithResultAndValidatorThatFails>("/result/validationFails")
                    .HttpFunction<HttpCommandWithResultAndValidatorThatPasses>("/result/validationPasses")
                )
                .HttpRoute("headers", route => route
                    .HttpFunction<HttpHeaderBindingCommand>()
                    .Options(options => options
                        .AddHeaderMapping(cmd => cmd.Value, "x-value")
                        .AddHeaderMapping(cmd => cmd.Message, "x-message")
                    )
                    .HttpFunction<HttpHeaderNullableValueTypeBindingCommand>("/nullableValueType")
                    .Options(options => options
                        .AddHeaderMapping(cmd => cmd.Value, "x-value")
                    )
                    .HttpFunction<HttpHeaderEnumTypeBindingCommand>("/enumType")
                    .Options(options => options
                        .AddHeaderMapping(cmd => cmd.Value, "x-enum-value")
                    )
                    .HttpFunction<HttpDefaultHeaderCommand>("/default")
                )
                .HttpRoute("responseHandler", route => route
                    .HttpFunction<HttpResponseHandlerCommandWithNoResultAndNoValidation>(
                        "/noResult/noValidation")
                    .Options(options => options.ResponseHandler<CustomResponseHandler>())
                    .HttpFunction<HttpResponseHandlerCommandWithNoResultAndValidatorThatFails>(
                        "/noResult/validationFails")
                    .Options(options => options.ResponseHandler<CustomResponseHandler>())
                    .HttpFunction<HttpResponseHandlerCommandWithNoResultAndValidatorThatPasses>(
                        "/noResult/validationPasses")
                    .Options(options => options.ResponseHandler<CustomResponseHandler>())
                    .HttpFunction<HttpResponseHandlerCommandWithResultAndNoValidation>("/result/noValidation")
                    .Options(options => options.ResponseHandler<CustomResponseHandler>())
                    .HttpFunction<HttpResponseHandlerCommandWithResultAndValidatorThatFails>(
                        "/result/validationFails")
                    .Options(options => options.ResponseHandler<CustomResponseHandler>())
                    .HttpFunction<HttpResponseHandlerCommandWithResultAndValidatorThatPasses>(
                        "/result/validationPasses")
                    .Options(options => options.ResponseHandler<CustomResponseHandler>())
                )

                .HttpRoute(route => route
                    .HttpFunction<HttpCommandWithNoRoute>()
                );
        }

        protected virtual IFunctionBuilder CreateAdditionalFunctions(IFunctionBuilder builder)
        {
            return builder;
        }

        public void Build(IFunctionHostBuilder builder)
        {
            builder
                .Setup((serviceCollection, commandRegistry) =>
                {
                    serviceCollection
                        .AddTransient<IMarker, Marker>()
                        .AddValidatorsFromAssemblyContaining<HttpCommandWithNoResultAndValidatorThatFailsValidator>()
                        ;
                })
                .AddFluentValidation()
                .OpenApiEndpoint(openApi => openApi
                    .UserInterface()
                    .Title("Integration Test Functions")
                    .Version("1.0.0")
                )
                .Authorization(authorization => authorization
                    .AuthorizationDefault(AuthorizationTypeEnum.Anonymous)
                    .TokenValidator<MockTokenValidator>()
                    .Claims(claims => claims
                        .MapClaimToPropertyName("claima", "StringClaim")
                        .MapClaimToCommandProperty<HttpIntClaimCommand>("claimb", cmd => cmd.MappedValue)
                    )
                )
                .DefaultConnectionStringSettingNames(settingNames =>
                {
                    // These are the default values - you don't have to set them
                    // I've set them here just to show what they are
                    settingNames.Storage = "storageConnectionString";
                    settingNames.CosmosDb = "cosmosConnectionString";
                    settingNames.ServiceBus = "serviceBusConnectionString";
                    settingNames.SignalR = "signalRConnectionString";
                })
                .DefaultHttpHeaderBindingConfiguration(new HeaderBindingConfiguration()
                {
                    PropertyFromHeaderMappings = new Dictionary<string, string>
                    {
                        {"DefaultHeaderIntValue", "x-default-int"},
                        {"DefaultHeaderStringValue", "x-default-string"}
                    }
                })
                .Functions(functions =>
                {
                    CreateCommonFunctions(functions);
                    CreateAdditionalFunctions(functions);
                });
        }
    }
}