using System;
using System.Text;

namespace FunctionMonkey.Extensions
{
    public static class Utils
    {
        public static string EvaluateType(Type type)
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
                return type.ToString().Replace("+", ".");
            }

            return retType.ToString().Replace("+", ".");
        }
    }
}
