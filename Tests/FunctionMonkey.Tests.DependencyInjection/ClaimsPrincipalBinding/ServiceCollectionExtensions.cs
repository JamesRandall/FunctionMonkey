namespace FunctionMonkey.Tests.DependencyInjection.ClaimsPrincipalBinding
{
	using System;
	using System.Security.Claims;

	using Castle.DynamicProxy;

	using Microsoft.Extensions.DependencyInjection;

	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddClaimsPrincipalBinding<TService>(
			this IServiceCollection services,
			Action<ClaimsPrincipalBindingOptions<TService>> binding) where TService : class
		{
			var options = new ClaimsPrincipalBindingOptions<TService>();
			binding(options);

			services.AddSingleton(s => new ProxyGenerator()
				.CreateInterfaceProxyWithoutTarget<TService>(
					new ClaimsPrincipalBindingInterceptor<TService>(
						() => s.GetRequiredService<ClaimsPrincipal>(),
						options)));

			return services;
		}
	}
}
