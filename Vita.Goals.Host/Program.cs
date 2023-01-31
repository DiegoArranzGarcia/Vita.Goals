﻿using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using Vita.Goals.Application.Commands.Goals;
using Vita.Goals.Application.Queries.Goals;
using Vita.Goals.Application.Queries.Tasks;
using Vita.Goals.Domain.Aggregates.Goals;
using Vita.Goals.Domain.Aggregates.Tasks;
using Vita.Goals.Infrastructure.Sql;
using Vita.Goals.Infrastructure.Sql.Aggregates.Goals;
using Vita.Goals.Infrastructure.Sql.Aggregates.Tasks;
using Vita.Goals.Infrastructure.Sql.QueryStores;
using Vita.Goals.Infrastructure.Sql.QueryStores.Configuration;

var builder = WebApplication.CreateBuilder(args);

string[] allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "spa-cors", builder =>
    {
        builder.WithOrigins(allowedOrigins.ToArray())
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

builder.Services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = builder.Configuration["JWTAuthority"];
                    options.RequireHttpsMetadata = false;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false
                    };
                });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiScope", policy =>
    {
        policy.RequireClaim("scope", "goals");
    });
});

AddApplicationBootstrapping();
AddPersistanceBootstrapping();

builder.Services.AddApplicationInsightsTelemetry(builder.Configuration["APPINSIGHTS_INSTRUMENTATIONKEY"]);

builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
    options.SerializerSettings.DateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ssZ";
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

void AddApplicationBootstrapping()
{
    builder.Services.AddMediatR(typeof(CreateGoalCommand), typeof(CreateGoalCommandHandler));
    builder.Services.AddSingleton<IConnectionStringProvider>(new ConnectionStringProvider(builder.Configuration.GetConnectionString("GoalsDbContext")));
}

void AddPersistanceBootstrapping()
{
    builder.Services.AddScoped<IGoalsRepository, GoalsRepository>();
    builder.Services.AddScoped<ITaskRepository, TaskRepository>();
    builder.Services.AddScoped<IGoalQueryStore, GoalQueryStore>();
    builder.Services.AddScoped<ITaskQueryStore, TaskQueryStore>();
    builder.Services.AddDbContext<GoalsDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("GoalsDbContext")));
}
