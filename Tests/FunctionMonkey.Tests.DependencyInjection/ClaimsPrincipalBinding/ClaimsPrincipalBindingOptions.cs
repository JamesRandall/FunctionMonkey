namespace FunctionMonkey.Tests.DependencyInjection.ClaimsPrincipalBinding
{
	using System;
	using System.Collections.Generic;
	using System.Linq.Expressions;

	public class ClaimsPrincipalBindingOptions<TService>
	{
		private readonly Dictionary<string, string> _configuration = new Dictionary<string, string>();

		public IReadOnlyDictionary<string, string> Configuration => _configuration;

		public ClaimsPrincipalBindingOptions<TService> ForProperty(Expression<Func<TService, string>> getProperty, string claimType)
		{
			var propertyName =
				getProperty.Body is UnaryExpression unaryExpression
				? ((MemberExpression)unaryExpression.Operand).Member.Name
				: ((MemberExpression)getProperty.Body).Member.Name;

			_configuration.Add(propertyName, claimType);

			return this;
		}
	}
}
