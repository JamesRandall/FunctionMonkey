using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace FunctionMonkey.Extensions
{
    internal static class TypeExtensions
    {
        public static bool IsSupportedQueryParameterType(this Type type)
        {
            return type.IsSupportedQueryParameterDiscreteType() ||
                   type.IsSupportedQueryParameterCollectionType();
        }

        private static bool IsSupportedQueryParameterDiscreteType(this Type type)
        {
            return type == typeof(string)
                   || type.GetMethods(BindingFlags.Public | BindingFlags.Static).Any(y => y.Name == "TryParse")
                   || type.IsEnum
                   || type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
        
        public static bool IsSupportedQueryParameterCollectionType(this Type type)
        {
            if (type == typeof(string))
            {
                return false;
            }
            
            Type enumerableType = type.GetEnumerableType();
            if (enumerableType != null)
            {
                Type enumerableValueType = enumerableType.GenericTypeArguments[0]; 
                if (enumerableValueType.IsSupportedQueryParameterDiscreteType())
                {
                    return 
                        type.IsArray || type.IsInterface ||
                        type.GetMethods().Any(x =>
                            x.Name == "Add" &&
                            x.GetParameters().Length == 1 &&
                            x.GetParameters()[0].ParameterType.IsAssignableFrom(enumerableValueType)
                        );
                }
            }

            return false;
        }

        public static Type SupportedCollectionValueType(this Type type)
        {
            Type genericEnumerableType = typeof(IEnumerable<>);
            Type enumerableType = type.GetEnumerableType();
            Debug.Assert(enumerableType != null);
            return enumerableType.GenericTypeArguments[0];
        }

        private static Type GetEnumerableType(this Type type)
        {
            Type genericEnumerableType = typeof(IEnumerable<>);
            Type enumerableType = type.GetInterfaces()
                .SingleOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == genericEnumerableType);
            if (enumerableType == null)
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == genericEnumerableType)
                {
                    enumerableType = type;
                }
            }

            return enumerableType;
        }
    }
}