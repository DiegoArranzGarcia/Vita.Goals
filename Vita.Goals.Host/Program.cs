using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Vita.Goals.Api;
using Vita.Goals.Application.Commands;
using Vita.Goals.Host.Infrastructure;
using Vita.Goals.Infrastructure.Sql;

var builder = WebApplication.CreateBuilder(args);

IServiceCollection services = builder.Services;
IConfiguration configuration = builder.Configuration;

services.AddCustomAuthentication(configuration);
services.AddCustomAuthorization();
services.AddCustomCors(configuration);

services.AddApplicationInsightsTelemetry(builder.Configuration["APPINSIGHTS_INSTRUMENTATIONKEY"]);

services.ConfigureApiServices();
services.ConfigureApplicationCommandServices();
services.ConfigurePersistenceServices(configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{   
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseCors("spa-cors");

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints => endpoints.MapControllers());

app.Run();
