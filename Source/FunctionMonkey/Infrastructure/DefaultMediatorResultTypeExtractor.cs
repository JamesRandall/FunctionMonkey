using System;
using System.Data;
using System.Linq;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions;

namespace FunctionMonkey.Infrastructure
{
    internal class DefaultMediatorResultTypeExtractor : IMediatorResultTypeExtractor
    {
        public Type CommandResultType(Type commandType)
        {
            Type commandInterface = typeof(ICommand);
            Type[] interfaces = commandType.GetInterfaces();
            Type[] minimalInterfaces = interfaces.Except(interfaces.SelectMany(i => i.GetInterfaces())).ToArray();
            Type genericCommandInterface = minimalInterfaces
                .SingleOrDefault(x => x.IsGenericType && commandInterface.IsAssignableFrom(x));

            if (genericCommandInterface != null)
            {
                return genericCommandInterface.GenericTypeArguments[0];
            }

            return null;
        }
    }
}