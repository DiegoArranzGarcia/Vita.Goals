using System.Security.Claims;
using Vita.Goals.FunctionalTests.Builders;

namespace Vita.Goals.FunctionalTests.Authentication;

public class UsersTokens : Dictionary<string, IReadOnlyCollection<Claim>>
{
	public const string AliceToken = nameof(AliceToken);
	public const string BobToken = nameof(BobToken);
	public const string UnauthorizedToken = nameof(UnauthorizedToken);

	public UsersTokens()
	{
		Add(AliceToken, UserBuilder.AliceClaims);
		Add(BobToken, UserBuilder.BobClaims);
		Add(UnauthorizedToken, UserBuilder.UnauthorizedUserClaims);
	}
}
