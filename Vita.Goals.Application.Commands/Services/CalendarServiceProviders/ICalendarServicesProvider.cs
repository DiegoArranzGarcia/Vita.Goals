using System;
using System.Threading.Tasks;

namespace Vita.Goals.Application.Commands.Services.CalendarServiceProviders;

public interface ICalendarServicesProvider
{

    Task CreateCalendar(Guid userId);
    Task DeleteCalendar(Guid userId);
}
