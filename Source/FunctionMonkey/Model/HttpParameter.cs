using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Http;

namespace FunctionMonkey.Model
{
    public class HttpParameter
    {
        public string Name { get; set; }

        public string TypeName { get; set; }

        public bool IsString => TypeName.Equals("System.String");

        public Type Type { get; set; }

        public bool IsFormCollection => Type == typeof(IFormCollection);

        public bool IsEnum => Type.IsEnum;

      public bool IsNullable => Type.IsGenericType && Type.GetGenericTypeDefinition() == typeof(Nullable<>);

	    public string NullableType => Type.GetGenericArguments().First().FullName;

	    public bool IsNullableTypeHasTryParseMethod => IsNullable && Type.GetGenericArguments().First()
		                                                   .GetMethods(BindingFlags.Public | BindingFlags.Static)
		                                                   .Any(x => x.Name == "TryParse");
	}
}
