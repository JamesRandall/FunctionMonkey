using System.Security.Claims;
using System.Threading.Tasks;
using FunctionMonkey.Abstractions;

namespace FunctionMonkey.Tests.Integration.Functions
{
    public class MockTokenValidator : ITokenValidator
    {
        public Task<ClaimsPrincipal> ValidateAsync(string authorizationHeader)
        {
            return Task.FromResult(new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim("claima", "a message"),
                        new Claim("claimb", "42"),
                    }
                )
            ));
        }
    }
}