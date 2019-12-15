using System;
using System.Reflection;
using FmFsharpDemo;
using Microsoft.FSharp.Collections;

namespace FSharpDiscoveryExample
{
    class Program
    {
        static void Main(string[] args)
        {
            Microsoft.FSharp.Collections.FSharpList<string> coll = FSharpList<string>.Empty;
            coll = new FSharpList<string>("newvalue", coll);
            Microsoft.FSharp.Core.FSharpOption<string> some = new Microsoft.FSharp.Core.FSharpOption<string>("ss");
            Microsoft.FSharp.Core.FSharpOption<string> option = Microsoft.FSharp.Core.FSharpOption<string>.None;
            //Microsoft.FSharp.Core.Unit
            /*Assembly assembly = typeof(FmFsharpDemo.EntryPoint.Order).Assembly;
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
            }*/
        }
    }
}
