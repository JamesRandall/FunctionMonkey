namespace FunctionMonkey.Tests.DependencyInjection.Infrastructure
{
	using System;
	using System.Security.Claims;
	using System.Threading.Tasks;

	using FunctionMonkey.Abstractions;

	public class TokenValidator : ITokenValidator
	{
		public Task<ClaimsPrincipal> ValidateAsync(string authorizationHeader)
		{
			return Task.FromResult(new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("sub", authorizationHeader), new Claim("name", $"name {authorizationHeader}") }, "valid")));
		}
	}
}
