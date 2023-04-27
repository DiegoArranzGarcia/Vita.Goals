using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Vita.Goals.FunctionalTests.Authentication;
using Vita.Goals.FunctionalTests.Builders;

namespace Vita.Goals.FunctionalTests.Middleware;

public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger, 
        UrlEncoder encoder, 
        ISystemClock clock) : base(options, logger, encoder, clock)
    {

    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // value is false (or anything other than 'true'). Make sure they've made the effort to provide the auth header 
        // but don't bother to check the value
        if (!Context.Request.Headers.TryGetValue("Authorization", out var authHeader))
            return Task.FromResult(AuthenticateResult.NoResult());

        // otherwise, here's your auth ticket
        string token = authHeader.First()![("Bearer ".Length)..];
        var claimsDictionary = new UsersTokens();

        if (!claimsDictionary.TryGetValue(token, out IReadOnlyCollection<Claim>? claims))
            return Task.FromResult(AuthenticateResult.NoResult());

        var identity = new ClaimsIdentity(claims, "TestAuthenticationScheme");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "TestAuthenticationScheme");

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}