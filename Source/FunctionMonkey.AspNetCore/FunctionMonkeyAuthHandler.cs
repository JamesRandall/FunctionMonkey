using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using FunctionMonkey.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FunctionMonkey.AspNetCore
{
    public class FunctionMonkeyAuthHandler : AuthenticationHandler<AuthenticationOptions>
    {
        private readonly ITokenValidator _tokenValidator;

        public FunctionMonkeyAuthHandler(ITokenValidator tokenValidator,
            IOptionsMonitor<AuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock) : base(options, logger, encoder, clock)
        {
            _tokenValidator = tokenValidator;
        }
        
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string header = Request.Headers["Authorization"];
            if (string.IsNullOrWhiteSpace(header))
            {
                return AuthenticateResult.NoResult();
            }
            
            ClaimsPrincipal principal = await _tokenValidator.ValidateAsync(Request.Headers["Authorization"]);
            if (principal == null)
            {
                return AuthenticateResult.Fail("Invalid token");
            }

            return AuthenticateResult.Success(
                new AuthenticationTicket(
                    principal,
                    new AuthenticationProperties(),
                    TokenValidationDefaults.AuthenticationScheme));
        }

        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            var authResult = await HandleAuthenticateOnceSafeAsync();
            if (!authResult.Succeeded)
            {
                Response.StatusCode = 401;
            }
        }
        
        /*protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
        {
            Response.StatusCode = 403;
            return Task.CompletedTask;
        }*/
    }
}