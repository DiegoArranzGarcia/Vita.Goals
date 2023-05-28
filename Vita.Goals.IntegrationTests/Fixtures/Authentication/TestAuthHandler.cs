using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Vita.Goals.FunctionalTests.Fixtures.Authentication;

public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string AuthenticationScheme = "TestAuthenticationScheme";

    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock) : base(options, logger, encoder, clock)
    {

    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Context.Request.Headers.TryGetValue("Authorization", out var authHeader))
            return Task.FromResult(AuthenticateResult.NoResult());

        string token = authHeader.First()!["Bearer ".Length..];
        IEnumerable<Claim> claims = Decode(token).ToList();

        var identity = new ClaimsIdentity(claims, AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, AuthenticationScheme);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    public static IEnumerable<Claim> Decode(string encodedValue)
    {
        if (string.IsNullOrEmpty(encodedValue))
            return Enumerable.Empty<Claim>();

        var serializer = new TicketSerializer();
        var ticket = serializer.Deserialize(Convert.FromBase64String(encodedValue));
        return ticket.Principal.Claims;
    }
}