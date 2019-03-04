using System.Security.Claims;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions;

namespace FunctionMonkey.Testing.Mocks
{
    public class CommandClaimsBinderMock : ICommandClaimsBinder
    {
        public bool Bind(ClaimsPrincipal principal, ICommand command)
        {
            return false;
        }
    }
}
