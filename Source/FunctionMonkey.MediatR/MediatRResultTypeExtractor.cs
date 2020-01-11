using System;
using System.Linq;
using FunctionMonkey.Abstractions;
using MediatR;

namespace FunctionMonkey.MediatR
{
    public class MediatRResultTypeExtractor : IMediatorResultTypeExtractor
    {
        public Type CommandResultType(Type commandType)
        {
            Type commandInterface = typeof(IRequest<>);
            Type[] interfaces = commandType.GetInterfaces();
            Type[] minimalInterfaces = interfaces.Except(interfaces.SelectMany(i => i.GetInterfaces())).ToArray();
            Type genericCommandInterface = minimalInterfaces
                .SingleOrDefault(x => x.IsGenericType && commandInterface.IsAssignableFrom(x.GetGenericTypeDefinition()));

            if (genericCommandInterface != null)
            {
                return genericCommandInterface.GenericTypeArguments[0];
            }

            return null;
        }
    }
}