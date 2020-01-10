using System;
using System.Linq;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions;

namespace FunctionMonkey.Infrastructure
{
    public class DefaultMediatorTypeSafetyEnforcer : IMediatorTypeSafetyEnforcer
    {
        public bool IsValidType(Type commandType)
        {
            return
                commandType.GetInterfaces().Any(x => x == typeof(ICommand)) ||
                commandType.GetInterfaces().Any(x =>
                    x.IsGenericTypeDefinition && x.GetGenericTypeDefinition() == typeof(ICommand<>));
        }

        public string Requirements => "Commands must implement ICommand or ICommand<T>";
    }
}