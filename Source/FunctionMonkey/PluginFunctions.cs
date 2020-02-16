using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using FunctionMonkey.Commanding.Abstractions.Validation;
using Microsoft.AspNetCore.Mvc;

namespace FunctionMonkey
{
    public class PluginFunctions : AbstractPluginFunctions
    {
        public Func<string, Task<ClaimsPrincipal>> ValidateToken { get; set; }

        public Func<ClaimsPrincipal, string, string, Task<bool>> IsAuthorized { get; set; }
        
        public Func<string, bool, object> Deserialize { get; set; }
        
        public Func<object, bool, string> Serialize { get; set; }
        
        public Func<ClaimsPrincipal, object, Task<object>> BindClaims { get; set; }
        
        public Func<object, Exception, Task<IActionResult>> CreateResponseFromException { get; set; }
        
        public Func<object, object, Task<IActionResult>> CreateResponseForResult { get; set; }
        
        public Func<object, Task<IActionResult>> CreateResponse { get; set; }
        
        public Func<object, object, Task<IActionResult>> CreateValidationFailureResponse { get; set; }
        
        public Func<object, bool> IsValid { get; set; }
        
        public Func<object, object> Validate { get; set; }
        
        public Func<object, object, object> OutputBindingConverter { get; set; }
        
        public Func<object, object> CommandTransformer { get; set; }

        // This is a func cast to an object that, when set, will be used to execute the command instead of the
        // built in dispatcher
        public object Handler { get; set; }
    }
}