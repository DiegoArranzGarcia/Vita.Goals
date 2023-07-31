using AutoMapper;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Reflection;
using Vita.Goals.Api.Profiles.Authorization;
using Vita.Goals.Application.Commands;

namespace Vita.Goals.Api;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public static class Startup
{
    public static void ConfigureApiServices(this IServiceCollection services)
    {
        services.AddFastEndpoints();

        services.AddSwaggerDoc(settings =>
        {
            settings.Version = "v1";
            settings.Title = "Goals API";
            settings.Description = "Vita Goals API for managing goals and tasks";

            //var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";

            //options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            //options.EnableAnnotations();

            //options.CustomSchemaIds((type) => type.Name!.EndsWith("Dto") ? type.Name[..^3] : type.Name);
        });

        services.ConfigureApplicationCommandServices();

        services.AddAutoMapper(config => config.AddProfile(typeof(AuthorizationProfile)));
    }
}

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member