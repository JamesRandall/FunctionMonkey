using System;
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

            return shortCommandName;
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
                    retType.Append(includeNamespace ? currentType.ToString() : currentType.Name);
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
    }
}
