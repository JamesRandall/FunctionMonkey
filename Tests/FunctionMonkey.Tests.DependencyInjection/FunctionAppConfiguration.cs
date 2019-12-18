namespace FunctionMonkey.Tests.DependencyInjection
{
	using System.Net.Http;

	using FunctionMonkey.Abstractions;
	using FunctionMonkey.Abstractions.Builders;
	using FunctionMonkey.Extensions;
	using FunctionMonkey.Tests.DependencyInjection.ClaimsPrincipalBinding;
	using FunctionMonkey.Tests.DependencyInjection.Commands;
	using FunctionMonkey.Tests.DependencyInjection.Handlers;
	using FunctionMonkey.Tests.DependencyInjection.Infrastructure;
	using FunctionMonkey.Tests.DependencyInjection.Services;

	using Microsoft.Extensions.DependencyInjection;

	public class FunctionAppConfiguration : IFunctionAppConfiguration
	{
		public void Build(IFunctionHostBuilder builder)
		{
			builder
				.Setup((services, commandRegistry) =>
				{
					services.AddScoped<IDisposableService, DisposableService>();

					//services.AddLogging(loggingBuilder =>
					//{
					//	loggingBuilder.SetMinimumLevel(LogLevel.Trace);
					//});

					services.AddFunctionMonkeyClaimsPrincipal();

					commandRegistry.Register<TestCommandHandler>();

					services.AddClaimsPrincipalBinding<ICallContext>(binding => binding
						.ForProperty(x => x.Name, "name")
						.ForProperty(x => x.Subject, "sub"));

					//services.AddSingleton(options);
					//services.AddSingleton<Func<ClaimsPrincipal>>(s => () => FunctionMonkey.Runtime.FunctionClaimsPrincipal.Value);
				})
				.OutputAuthoredSource(@"y:\Git\Repos2\FunctionMonkey\Tests\FunctionMonkey.Tests.DependencyInjection\bin\_output")
				.Authorization(x => x
					.AuthorizationDefault(AuthorizationTypeEnum.TokenValidation)
					.TokenValidator<TokenValidator>())
				.Functions(functions =>
				{
					functions
						.HttpRoute("/api/v1/HelloWorld", route => route
							.HttpFunction<TestCommand>(HttpMethod.Get));
				});
		}
	}
}
