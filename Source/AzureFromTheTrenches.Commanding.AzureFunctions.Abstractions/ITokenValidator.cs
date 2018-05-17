using System.Security.Claims;
using System.Threading.Tasks;

namespace AzureFromTheTrenches.Commanding.AzureFunctions.Abstractions
{
    public interface ITokenValidator
    {
        Task<ClaimsPrincipal> ValidateAsync(string authorizationHeader);
    }
}
