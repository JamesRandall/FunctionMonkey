using System;
using System.Collections.Generic;
using System.Security.Claims;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions;

namespace FunctionMonkey.Infrastructure
{
    internal class CommandClaimsBinder : ICommandClaimsBinder
    {
        private readonly Dictionary<Type, Action<object, ClaimsPrincipal>> _mappers;

        public CommandClaimsBinder(Dictionary<Type, Action<object, ClaimsPrincipal>> mappers)
        {
            _mappers = mappers;
        }

        public bool Bind(ClaimsPrincipal principal, ICommand command)
        {
            if (_mappers.TryGetValue(command.GetType(), out var binder))
            {
                binder(command, principal);
                return true;
            }

            return false;
        }
    }
}
