using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace FunctionMonkey.Abstractions.Contexts
{
    public class HttpContext
    {
        public string RequestUrl { get; set; }

        public Dictionary<string, IReadOnlyCollection<string>> Headers { get; set; }
        
        public ClaimsPrincipal ClaimsPrincipal { get; set; }
    }
}
