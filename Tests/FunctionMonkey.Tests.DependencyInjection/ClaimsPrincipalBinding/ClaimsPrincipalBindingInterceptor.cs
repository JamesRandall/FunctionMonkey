namespace FunctionMonkey.Tests.DependencyInjection.ClaimsPrincipalBinding
{
	using System;
	using System.Security.Claims;
	using Castle.DynamicProxy;

	public class ClaimsPrincipalBindingInterceptor<TService> : IInterceptor
	{
		private readonly Func<ClaimsPrincipal> _principalFunc;
		private readonly ClaimsPrincipalBindingOptions<TService> _options;

		public ClaimsPrincipalBindingInterceptor(Func<ClaimsPrincipal> principalFunc, ClaimsPrincipalBindingOptions<TService> options)
		{
			_principalFunc = principalFunc;
			_options = options;
		}

		public void Intercept(IInvocation invocation)
		{
			if (invocation.Method.Name.StartsWith("get_"))
			{
				var propertyName = invocation.Method.Name.Substring(4);
				invocation.ReturnValue = _principalFunc().FindFirst(_options.Configuration[propertyName])?.Value;
			}
		}
	}
}
