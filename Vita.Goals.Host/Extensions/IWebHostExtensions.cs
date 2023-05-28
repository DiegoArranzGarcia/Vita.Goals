using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Vita.Goals.Host.Extensions;

public static class IWebHostExtensions
{
    public static WebApplication MigrateDbContext<TContext>(this WebApplication webApplication) where TContext : DbContext
    {
        using (var scope = webApplication.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var context = services.GetService<TContext>();

            context.Database.Migrate();
        }

        return webApplication;
    }
}
