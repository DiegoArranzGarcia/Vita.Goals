using FastEndpoints;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Vita.Goals.Application.Commands;

using FastEndpoints.Swagger; 

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
    }
}

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member