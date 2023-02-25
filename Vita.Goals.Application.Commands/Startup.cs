using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Vita.Goals.Application.Commands.Goals;

namespace Vita.Goals.Application.Commands;

public static class Startup
{
    public static void ConfigureApplicationCommandServices(this IServiceCollection services)
    {
        services.AddMediatR(typeof(CreateGoalCommand), typeof(CreateGoalCommandHandler));
    }
}
