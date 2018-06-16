using System.Security.Claims;
using System.Threading.Tasks;

namespace FunctionMonkey.Abstractions
{
    /// <summary>
    /// This can be implemented to provide Authorization header validation / verification and set claims for HTTP triggers.
    /// See this gist for an example: https://gist.github.com/JamesRandall/e83f72f98bde2f6ff973e6ecb81199c8 
    /// </summary>
    public interface ITokenValidator
    {
        Task<ClaimsPrincipal> ValidateAsync(string authorizationHeader);
    }
}
