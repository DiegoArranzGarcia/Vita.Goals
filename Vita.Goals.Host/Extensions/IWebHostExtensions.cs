using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

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
