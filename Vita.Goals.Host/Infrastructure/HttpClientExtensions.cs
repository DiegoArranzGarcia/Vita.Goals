using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Vita.Goals.Infrastructure.Clients.Identity;

namespace Vita.Goals.Host.Infrastructure;

public static class HttpClientExtensions
{
    private const string TokenClientName = "VitaApiTokenClient";

    public static IServiceCollection RegisterVitaHttpClients(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddClientAccessTokenManagement(options =>
        {
            options.Clients.Add(TokenClientName, new ClientCredentialsTokenRequest
            {
                Address = $"{configuration["JWTAuthority"]}/connect/token",
                ClientId = configuration["Clients:Server2Server:ClientId"],
                ClientSecret = configuration["Clients:Server2Server:ClientSecret"],
                Scope = "identity goals"
            });
        });


        services.AddHttpClient<VitaIdentityApiClient>(client => { client.BaseAddress = new Uri("https://localhost:44360"); })
                .AddClientAccessTokenHandler(TokenClientName);

        return services;
    }
}
