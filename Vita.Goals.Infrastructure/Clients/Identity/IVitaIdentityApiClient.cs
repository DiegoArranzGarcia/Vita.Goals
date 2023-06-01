namespace Vita.Goals.Infrastructure.Clients.Identity;

public interface IVitaIdentityApiClient
{
    Task<IEnumerable<LoginProviderDto>> GetExternalLoginProviders(Guid userId);
    Task<AccessTokenDto> GetLoginProviderUserAccessToken(Guid userId, Guid loginProviderId);
}