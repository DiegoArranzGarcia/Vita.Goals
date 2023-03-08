using MediatR;
using System;

namespace Vita.Goals.Application.Commands.Calendars.CreateCalendar;

public record CreateCalendarCommand(Guid UserId, string ProviderName) : IRequest;