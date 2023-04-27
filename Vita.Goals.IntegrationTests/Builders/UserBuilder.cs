using System.Security.Claims;

namespace Vita.Goals.FunctionalTests.Builders;
public class UserBuilder
{
    public static readonly Guid AliceUserId = new("4e871c74-248f-45e3-b5f2-72c94cb48753");
    public static readonly Guid BobUserId = new("24120ca6-813d-4371-b40a-4e5b096b6cb6");
    public static readonly Guid UnauthorizedUserId = new("eba5b9fa-0180-4c74-b964-213ec82eb02b");

    public static readonly IReadOnlyCollection<Claim> AliceClaims = new[]
    {
        new Claim(ClaimTypes.Name, "Alice"),
        new Claim(ClaimTypes.NameIdentifier, AliceUserId.ToString()),
        new Claim("scope", "goals")
    };

    public static readonly IReadOnlyCollection<Claim> BobClaims = new[]
    {
        new Claim(ClaimTypes.Name, "Bob"),
        new Claim(ClaimTypes.NameIdentifier, BobUserId.ToString()),
        new Claim("scope", "goals")
    };

    public static readonly IReadOnlyCollection<Claim> UnauthorizedUserClaims = new[]
    {
        new Claim(ClaimTypes.Name, "UnauthorizedUserId"),
        new Claim(ClaimTypes.NameIdentifier, UnauthorizedUserId.ToString()),
    };
}
