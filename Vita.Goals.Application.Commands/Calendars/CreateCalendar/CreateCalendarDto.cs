using System;

namespace Vita.Goals.Application.Commands.Calendars.CreateCalendar;

public record CreateCalendarDto(Guid UserId, string ProviderName);
