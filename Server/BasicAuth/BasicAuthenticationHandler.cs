using System;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Server.Tools;

namespace Server.BasicAuth
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IServiceProvider services;
        private readonly Settings settings;

        public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IServiceProvider services) : base(options, logger, encoder, clock)
        {
            this.services = services;
            settings = this.services.GetRequiredService<Settings>();
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            Response.Headers.Add("WWW-Authenticate", "Basic");

            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return Task.FromResult(AuthenticateResult.Fail("Authorization header missing."));
            }

            string authorizationHeader = Request.Headers["Authorization"].ToString();
            Regex authHeaderRegex = new Regex(@"Basic (.*)");

            if (!authHeaderRegex.IsMatch(authorizationHeader))
            {
                return Task.FromResult(AuthenticateResult.Fail("Authorization code not formatted properly."));
            }

            string authBase64 = Encoding.UTF8.GetString(Convert.FromBase64String(authHeaderRegex.Replace(authorizationHeader, "$1")));
            string[] authSplit = authBase64.Split(Convert.ToChar(":"), 2);
            string authUsername = authSplit[0];
            string authPassword = authSplit.Length > 1 ? authSplit[1] : throw new Exception("Unable to get password");

            if (authUsername != settings.AuthName || authPassword != settings.AuthPassword)
            {
                return Task.FromResult(AuthenticateResult.Fail("The username or password is not correct."));
            }

            AuthenticatedUser authenticatedUser = new AuthenticatedUser("BasicAuthentication", true, settings.AuthName);
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(authenticatedUser));

            return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name)));
        }
    }
}
