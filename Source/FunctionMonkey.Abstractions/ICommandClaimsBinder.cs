using System.Security.Claims;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Abstractions
{
    /// <summary>
    /// Implementations of this interface lift claims from the a authenticated user and map them
    /// onto the properties of a command
    /// </summary>
    public interface ICommandClaimsBinder
    {
        /// <summary>
        /// Binds the claims in the principal onto properties of the command
        /// </summary>
        object Bind(ClaimsPrincipal principal, object command);        
        
        /// <summary>
        /// Binds the claims in the principal onto properties of the command. If this returns null then
        /// the non-async binder will be called.
        /// </summary>
        Task<object> BindAsync(ClaimsPrincipal principal, object command);
    }
}
