using System;
using System.Text;

namespace FunctionMonkey.Extensions
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

            if (type.IsGenericType)
            {
                string[] parentType = type.FullName.Split('`');
                Type[] arguments = type.GetGenericArguments();

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
                return type.ToString();
            }

            return retType.ToString();
        }
    }
}
