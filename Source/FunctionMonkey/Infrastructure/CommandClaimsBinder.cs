using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions;

namespace FunctionMonkey.Infrastructure
{
    internal class CommandClaimsBinder : ICommandClaimsBinder
    {
        private readonly Dictionary<Type, Func<object, ClaimsPrincipal, object>> _mappers;

        public CommandClaimsBinder(Dictionary<Type, Func<object, ClaimsPrincipal, object>> mappers)
        {
            _mappers = mappers;
        }

        public object Bind(ClaimsPrincipal principal, object command)
        {
            if (_mappers.TryGetValue(command.GetType(), out var binder))
            {
                return binder(command, principal);
            }

            return command;
        }
        
        public Task<object> BindAsync(ClaimsPrincipal principal, object command)
        {
            if (_mappers.TryGetValue(command.GetType(), out var binder))
            {
                return Task.FromResult(binder(command, principal));
            }

            return Task.FromResult(command);
        }
    }
}
