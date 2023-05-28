﻿using Microsoft.Extensions.DependencyInjection;
using Vita.Goals.Application.Commands.Goals;
using Vita.Goals.Application.Commands.Services.CalendarServiceProviders;

namespace Vita.Goals.Application.Commands;

public static class Startup
{
    public static void ConfigureApplicationCommandServices(this IServiceCollection services)
    {
        services.AddScoped<CalendarServicesProviderFactory>();

        services.AddMediatR((configuration) => configuration.RegisterServicesFromAssemblyContaining<CreateGoalCommand>());
    }
}
