using System.Security.Claims;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Abstractions
{
    public interface ICommandClaimsBinder
    {
        bool Bind(ClaimsPrincipal principal, ICommand command);        
    }
}
