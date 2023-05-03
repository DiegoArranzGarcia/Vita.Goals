using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.TestHost;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Security.Claims;
using Vita.Goals.FunctionalTests.Fixtures.Authentication;

namespace Vita.Goals.FunctionalTests.Fixtures.Extensions;

public static class RequestBuilderExtensions
{
    public static RequestBuilder WithIdentity(this RequestBuilder builder, IEnumerable<Claim> claims)
    {
        string encodedClaims = Encode(claims);

        return builder.AddHeader("Authorization", $"Bearer {encodedClaims}");
    }

    public static HttpClient WithIdentity(this HttpClient httpClient, IEnumerable<Claim> claims)
    {
        string encodedClaims = Encode(claims);

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, encodedClaims);

        return httpClient;
    }

    private static string Encode(IEnumerable<Claim> claims)
    {
        var ticket = new AuthenticationTicket
        (
            principal: new ClaimsPrincipal(new ClaimsIdentity(claims)),
            authenticationScheme: TestAuthHandler.AuthenticationScheme
        );

        var serializer = new TicketSerializer();
        var bytes = serializer.Serialize(ticket);

        return Convert.ToBase64String(bytes);
    }
}
