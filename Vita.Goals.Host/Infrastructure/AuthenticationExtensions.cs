using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Vita.Goals.Host.Infrastructure;

public static class AuthenticationExtensions
{
    private const string BearerSchemeName = "Bearer";

    public static AuthenticationBuilder AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddAuthentication(defaultScheme: BearerSchemeName)
                       .AddJwtBearer(BearerSchemeName, options =>
                       {
                           options.Authority = configuration["JWTAuthority"];
                           options.RequireHttpsMetadata = false;

                           options.TokenValidationParameters = new TokenValidationParameters
                           {
                               ValidateAudience = false
                           };
                       });
    }
}
