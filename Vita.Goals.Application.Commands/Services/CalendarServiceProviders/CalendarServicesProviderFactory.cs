using Microsoft.Extensions.DependencyInjection;
using System;
using Vita.Goals.Infrastructure.Clients.Identity;

namespace Vita.Goals.Application.Commands.Services.CalendarServiceProviders;

public class CalendarServicesProviderFactory
{
    private readonly IServiceProvider _serviceProvider;

    public CalendarServicesProviderFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ICalendarServicesProvider CreateCalendarServicesProvider(LoginProviderDto loginProvider)
    {
        return loginProvider.Name switch
        {
            "Google" => ActivatorUtilities.CreateInstance<GoogleCalendarServiceProvider>(_serviceProvider, new object[] { loginProvider }),

            _ => throw new NotImplementedException($"Not implemented calendar services for provider {loginProvider}"),
        };
    }
}
