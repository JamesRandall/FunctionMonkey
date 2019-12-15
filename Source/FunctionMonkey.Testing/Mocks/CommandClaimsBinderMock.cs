using System.Security.Claims;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions;

namespace FunctionMonkey.Testing.Mocks
{
    public class CommandClaimsBinderMock : ICommandClaimsBinder
    {
        public object Bind(ClaimsPrincipal principal, object command)
        {
            return command;
        }

        public Task<object> BindAsync(ClaimsPrincipal principal, object command)
        {
            return Task.FromResult(command);
        }
    }
}
