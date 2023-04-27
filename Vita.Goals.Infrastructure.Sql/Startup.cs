using Microsoft.Extensions.DependencyInjection;
using Vita.Goals.Application.Queries.Goals;
using Vita.Goals.Application.Queries.Tasks;
using Vita.Goals.Domain.Aggregates.Goals;
using Vita.Goals.Domain.Aggregates.Tasks;
using Vita.Goals.Infrastructure.Sql.Aggregates.Goals;
using Vita.Goals.Infrastructure.Sql.Aggregates.Tasks;
using Vita.Goals.Infrastructure.Sql.QueryStores.Configuration;
using Vita.Goals.Infrastructure.Sql.QueryStores;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Vita.Goals.Infrastructure.Sql;

public static class Startup
{
    public static void ConfigurePersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("GoalsDbContext");
        services.AddSingleton<IConnectionStringProvider>(new ConnectionStringProvider(connectionString));

        services.AddScoped<IGoalsRepository, GoalsRepository>();
        services.AddScoped<ITaskRepository, TaskRepository>();

        services.AddScoped<IGoalQueryStore, GoalQueryStore>();
        services.AddScoped<ITaskQueryStore, TaskQueryStore>();

        services.AddDbContext<GoalsDbContext>(options => options.UseSqlServer(connectionString, sqlOptions => sqlOptions.MigrationsAssembly(typeof(Startup).Assembly.GetName().Name)));
    }
}
