using System;
using System.Linq;
using FunctionMonkey.Abstractions;
using MediatR;

namespace FunctionMonkey.MediatR
{
    public class MediatRTypeSafetyEnforcer : IMediatorTypeSafetyEnforcer
    {
        public bool IsValidType(Type commandType)
        {
            return
                commandType.GetInterfaces().Any(x => x == typeof(INotification)) ||
                commandType.GetInterfaces().Any(x =>
                    x.IsGenericTypeDefinition && x.GetGenericTypeDefinition() == typeof(IRequest<>));
        }
    }
}