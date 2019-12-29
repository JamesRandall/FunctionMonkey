using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using FunctionMonkey.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using HttpResponse = Microsoft.AspNetCore.Http.HttpResponse;

namespace FunctionMonkey.AspNetCore
{
    public class FunctionMonkeyAuthHandler : IAuthenticationHandler
    {
        private readonly ITokenValidator _tokenValidator;

        public FunctionMonkeyAuthHandler(ITokenValidator tokenValidator)
        {
            _tokenValidator = tokenValidator;
        }
        
        protected HttpContext Context { get; private set; }

        protected HttpRequest Request
        {
            get => Context.Request;
        }

        protected HttpResponse Response
        {
            get => Context.Response;
        }
        
        public async Task<AuthenticateResult> AuthenticateAsync()
        {
            ClaimsPrincipal principal = await _tokenValidator.ValidateAsync(Request.Headers["Authorization"]);
            if (principal == null)
            {
                return AuthenticateResult.Fail("Invalid token");
            }

            return AuthenticateResult.Success(
                new AuthenticationTicket(
                    principal,
                    "Bearer"));
        }

        public Task ChallengeAsync(AuthenticationProperties properties)
        {
            Response.StatusCode = 401;
            return Task.CompletedTask;
        }

        public Task ForbidAsync(AuthenticationProperties properties)
        {
            Response.StatusCode = 403;
            return Task.CompletedTask;
        }

        public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
        {
            if (scheme.Name != "Bearer")
            {
                throw new NotSupportedException();
            }
            Context = context;
            return Task.CompletedTask;
        }
    }
}