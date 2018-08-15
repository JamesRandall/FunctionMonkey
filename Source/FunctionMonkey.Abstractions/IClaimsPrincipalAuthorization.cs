using System.Security.Claims;
using System.Threading.Tasks;

namespace FunctionMonkey.Abstractions
{
    /// <summary>
    /// Can be implemented to provide authorization based on a claims principal, a verb and a url
    /// </summary>
    public interface IClaimsPrincipalAuthorization
    {
        /// <summary>
        /// Should return true if the principal is authorized to access the resource, false if not
        /// </summary>
        /// <param name="claimsPrincipal">The claims principal</param>
        /// <param name="httpVerb">The verb used to access the request</param>
        /// <param name="requestUrl">The request URL</param>
        /// <returns></returns>
        Task<bool> IsAuthorized(ClaimsPrincipal claimsPrincipal, string httpVerb, string requestUrl);
    }
}
