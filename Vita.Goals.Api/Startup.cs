using Microsoft.Extensions.DependencyInjection;

namespace Vita.Goals.Api;

public static class Startup
{
    public static void ConfigureApiServices(this IServiceCollection services)
    {
        services.AddControllers();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }
}
