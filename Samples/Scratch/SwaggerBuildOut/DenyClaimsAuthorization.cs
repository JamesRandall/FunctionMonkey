using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using FunctionMonkey.Abstractions;

namespace SwaggerBuildOut
{
    public class DenyClaimsAuthorization : IClaimsPrincipalAuthorization
    {
        public Task<bool> IsAuthorized(ClaimsPrincipal claimsPrincipal, string httpVerb, string url)
        {
            return Task.FromResult(false);
        }
    }

    public class AllowClaimsAuthorization : IClaimsPrincipalAuthorization
    {
        public Task<bool> IsAuthorized(ClaimsPrincipal claimsPrincipal, string httpVerb, string url)
        {
            return Task.FromResult(true);
        }
    }
}
