using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Vita.Goals.Api;
using Vita.Goals.Host.Extensions;
using Vita.Goals.Host.Infrastructure;
using Vita.Goals.Infrastructure.Sql;

var builder = WebApplication.CreateBuilder(args);

IServiceCollection services = builder.Services;
IConfiguration configuration = builder.Configuration;

services.AddCustomAuthentication(configuration);
services.AddCustomAuthorization();
services.AddCustomCors(configuration);

services.ConfigureApiServices();
services.ConfigurePersistenceServices(configuration);

services.AddApplicationInsightsTelemetry(configuration["APPINSIGHTS_INSTRUMENTATIONKEY"]);

services.RegisterVitaHttpClients(configuration);

var app = builder.Build();

app.UseFastEndpoints(config =>
{
    config.Endpoints.RoutePrefix = "api";
    config.Endpoints.Configurator = endpointBuilder =>
    {
        endpointBuilder.DontCatchExceptions();
    };
});

app.UseCustomExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwaggerGen();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
        options.DisplayOperationId();
    });
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseCors("spa-cors");

app.UseAuthentication();
app.UseAuthorization();

app.MigrateDbContext<GoalsDbContext>();

app.Run();

public partial class Program
{

}