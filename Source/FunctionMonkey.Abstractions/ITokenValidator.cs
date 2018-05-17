using System.Security.Claims;
using System.Threading.Tasks;

namespace FunctionMonkey.Abstractions
{
    public interface ITokenValidator
    {
        Task<ClaimsPrincipal> ValidateAsync(string authorizationHeader);
    }
}
