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
                        typeof(FunctionMonkey.FSharp.Configuration.FunctionAppConfiguration))
                    {
                        FunctionMonkey.FSharp.Configuration.FunctionAppConfiguration configuration2 =
                            (FunctionMonkey.FSharp.Configuration.FunctionAppConfiguration)propertyInfo.GetValue(null);
                    }
                }
            }
        }
    }
}
