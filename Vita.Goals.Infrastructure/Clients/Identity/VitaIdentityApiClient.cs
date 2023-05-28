using System.Net.Http.Json;

namespace Vita.Goals.Infrastructure.Clients.Identity;

public class VitaIdentityApiClient
{
    private readonly HttpClient _httpClient;

    public VitaIdentityApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<LoginProviderDto>> GetExternalLoginProviders(Guid userId)
    {
        var response = await _httpClient.GetAsync($"https://localhost:44360/api/users/{userId}/login-providers");

        if (!response.IsSuccessStatusCode)
            throw new Exception($"An error ocurred when getting the login providers for user {userId}");

        return await response.Content.ReadFromJsonAsync<IEnumerable<LoginProviderDto>>();
    }

    public async Task<AccessTokenDto> GetLoginProviderUserAccessToken(Guid userId, Guid loginProviderId)
    {
        var response = await _httpClient.GetAsync($"https://localhost:44360/api/users/{userId}/login-providers/{loginProviderId}/access-token");

        if (!response.IsSuccessStatusCode)
            throw new Exception($"An error ocurred when getting the access token of the user {userId} from the provider {loginProviderId}");

        return await response.Content.ReadFromJsonAsync<AccessTokenDto>();
    }
}
