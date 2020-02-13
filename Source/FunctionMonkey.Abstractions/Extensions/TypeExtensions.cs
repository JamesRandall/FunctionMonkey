using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FunctionMonkey.Abstractions.Extensions
{
    public static class TypeExtensions
    {
        public static string GetFunctionName(this Type commandType)
        {
            string shortCommandName = commandType.Name;

            if (shortCommandName.EndsWith("Query"))
            {
                return shortCommandName.Substring(0, shortCommandName.Length - 5);
            }
            if (shortCommandName.EndsWith("Command"))
            {
                return shortCommandName.Substring(0, shortCommandName.Length - 7);
            }

            // this deals with generic function types
            return shortCommandName.Replace(@"`", "__");
        }

        public static string EvaluateType(this Type type)
        {
            StringBuilder retType = new StringBuilder();

            void ExpandType(Type currentType, bool includeNamespace)
            {
                if (currentType.IsGenericType)
                {
                    string[] parentType = (includeNamespace ? currentType.FullName : currentType.Name).Split('`');
                    Type[] arguments = currentType.GetGenericArguments();

                    StringBuilder argList = new StringBuilder();
                    foreach (Type t in arguments)
                    {
                        string arg = EvaluateType(t);
                        if (argList.Length > 0)
                        {
                            argList.AppendFormat(", {0}", arg);
                        }
                        else
                        {
                            argList.Append(arg);
                        }
                    }

                    if (argList.Length > 0)
                    {
                        retType.AppendFormat("{0}<{1}>", parentType[0], argList.ToString());
                    }
                }
                else
                {
                    // Not sure if this is correct, its to deal with F# array types as in the ToDo example
                    if (currentType.IsArray && currentType.ToString().Contains('+'))
                    {
                        retType.Append(includeNamespace ? currentType.ToString().Replace("+", ".") : currentType.Name);
                    }
                    else
                    {
                        retType.Append(includeNamespace ? currentType.ToString() : currentType.Name);
                    }
                }
            }
            
            void RecurseUpDeclaringTypes(Type currentType)
            {
                if (currentType.DeclaringType == null)
                {
                    ExpandType(currentType, true);
                }
                else
                {
                    RecurseUpDeclaringTypes(currentType.DeclaringType);
                    retType.Append(".");
                    ExpandType(currentType, false);
                }
            }
            
            RecurseUpDeclaringTypes(type);
            

            return retType.ToString();
        }
        
        public static bool IsSupportedCSharpQueryParameterType(this Type type)
        {
            return type.IsSupportedQueryParameterDiscreteType() ||
                   type.IsSupportedCSharpQueryParameterCollectionType();
        }

        private static bool IsSupportedQueryParameterDiscreteType(this Type type)
        {
            return type == typeof(string)
                   || type.GetMethods(BindingFlags.Public | BindingFlags.Static).Any(y => y.Name == "TryParse")
                   || type.IsEnum
                   || type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
        
        public static bool IsSupportedCSharpQueryParameterCollectionType(this Type type)
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
