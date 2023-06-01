using Vita.Goals.Infrastructure.Clients.Identity;

namespace Vita.Goals.Application.Commands.Services.CalendarServiceProviders;
public interface ICalendarServicesProviderFactory
{
    ICalendarServicesProvider CreateCalendarServicesProvider(LoginProviderDto loginProvider);
}