using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Vita.Goals.Host.Infrastructure;

public static class AuthenticationExtensions
{
    public static AuthenticationBuilder AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                       .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
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
