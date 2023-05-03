using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Respawn;
using System.Data.Common;
using Testcontainers.MsSql;
using Vita.Goals.FunctionalTests.Fixtures.Authentication;
using Vita.Goals.Infrastructure.Sql;
using Vita.Goals.Infrastructure.Sql.QueryStores.Configuration;

namespace Vita.Goals.FunctionalTests.Fixtures.WebApplicationFactories;

public class WebApiApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public MsSqlContainer? MsSqlContainer { get; private set; }
    public Respawner? Respawner { get; private set; }

    private string ConnectionString => $"Server={MsSqlContainer!.Hostname},{MsSqlContainer.GetMappedPublicPort(1433)};Database=Vita.Goals.IntegrationTests;User Id=sa;Password=yourStrong(!)Password;TrustServerCertificate=True";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services => services.AddAuthentication(TestAuthHandler.AuthenticationScheme)
                                                          .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.AuthenticationScheme, _ => { }));

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<GoalsDbContext>>();
            services.RemoveAll<DbConnection>();

            services.AddDbContext<GoalsDbContext>(options =>
            {
                options.UseSqlServer(ConnectionString, sqloptions =>
                {
                    sqloptions.MigrationsAssembly(typeof(GoalsDbContext).Assembly.GetName().Name);
                });
            });

            services.RemoveAll<IConnectionStringProvider>();
            services.AddSingleton<IConnectionStringProvider>(new ConnectionStringProvider(ConnectionString));
        });

        builder.UseEnvironment("IntegrationTests");
    }

    public async Task InitializeAsync()
    {
        MsSqlContainer = new MsSqlBuilder().Build();
        await MsSqlContainer.StartAsync();
        await CreateDatabase();
        Respawner = await ConfigureRespawner();
    }

    private Task CreateDatabase()
    {
        IServiceScope scope = Services.CreateScope();

        IServiceProvider scopedServices = scope.ServiceProvider;
        GoalsDbContext context = scopedServices.GetRequiredService<GoalsDbContext>();

        return context.Database.EnsureCreatedAsync();
    }

    public GoalsDbContext GetGoalsDbContext()
    {
        IServiceScope scope = Services.CreateScope();
        IServiceProvider scopedServices = scope.ServiceProvider;

        return scopedServices.GetRequiredService<GoalsDbContext>();
    }

    private Task<Respawner> ConfigureRespawner()
    {
        return Respawner.CreateAsync(ConnectionString, new RespawnerOptions()
        {
            TablesToIgnore = new Respawn.Graph.Table[]
            {
                new("__EFMigrationsHistory"),
                new("GoalStatus"),
                new("TaskStatus")
            }
        });
    }

    Task IAsyncLifetime.DisposeAsync()
    {
        return MsSqlContainer!.StopAsync().ContinueWith(_ => MsSqlContainer.DisposeAsync().AsTask());
    }

    internal Task CleanDatabase()
    {
        return Respawner!.ResetAsync(ConnectionString);
    }
}