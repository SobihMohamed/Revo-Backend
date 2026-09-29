using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace Revo.IntegrationTests.Infrastructre
{
    public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string DefaultScheme = "TestScheme";
        // This is a test authentication handler that can be used in integration tests to simulate authentication.
        public TestAuthHandler(
                    IOptionsMonitor<AuthenticationSchemeOptions> options, // this is the options monitor that provides the authentication scheme options
                    ILoggerFactory logger,
                    UrlEncoder encoder // this is the URL encoder that is used to encode URLs
                   ) : base(options, logger, encoder) 
        {
        }
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[]
            {
               new Claim(ClaimTypes.NameIdentifier, "TestAdminId"),
                new Claim(ClaimTypes.Name, "TestAdmin"),
                new Claim(ClaimTypes.Role, "Admin")
            };
            // Create a ClaimsIdentity with the claims and the authentication scheme
            var identity = new ClaimsIdentity(claims, DefaultScheme); // this is the authentication scheme that is used to identify the authentication handler
            var principal = new ClaimsPrincipal(identity);// this is the principal that represents the authenticated user
            var ticket = new AuthenticationTicket(principal, DefaultScheme); // this is the authentication ticket that contains the principal and the authentication scheme
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
