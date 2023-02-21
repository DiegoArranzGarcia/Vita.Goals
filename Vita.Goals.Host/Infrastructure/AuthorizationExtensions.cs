using Microsoft.Extensions.DependencyInjection;

namespace Vita.Goals.Host.Infrastructure;

public static class AuthorizationExtensions
{
    public static IServiceCollection AddCustomAuthorization(this IServiceCollection services)
    {
        return services.AddAuthorization(options =>
        {
            options.AddPolicy("ApiScope", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("scope", "goals");
            });
        });
    }
}
