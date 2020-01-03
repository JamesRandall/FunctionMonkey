using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace FunctionMonkey.Abstractions
{
    public class TokenValidationResult
    {
        public ClaimsPrincipal Principal { get; set; }
        
        public SecurityToken ValidatedToken { get; set; }
    }
}