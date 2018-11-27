using System;
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
    }
}
