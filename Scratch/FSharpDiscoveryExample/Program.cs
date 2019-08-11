using System;
using System.Reflection;
using FmFsharpDemo;

namespace FSharpDiscoveryExample
{
    class Program
    {
        static void Main(string[] args)
        {
            
            
            Assembly assembly = typeof(FmFsharpDemo.EntryPoint.Order).Assembly;
            foreach (Type type in assembly.ExportedTypes)
            {
                foreach (PropertyInfo propertyInfo in type.GetProperties())
                {
                    if (propertyInfo.PropertyType ==
                        typeof(FunctionMonkey.FSharp.Models.FunctionAppConfiguration))
                    {
                        FunctionMonkey.FSharp.Models.FunctionAppConfiguration configuration2 =
                            (FunctionMonkey.FSharp.Models.FunctionAppConfiguration)propertyInfo.GetValue(null);
                    }
                }
            }
        }
    }
}
