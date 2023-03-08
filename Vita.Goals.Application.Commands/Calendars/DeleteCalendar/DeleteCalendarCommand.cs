using MediatR;
using System;

namespace Vita.Goals.Application.Commands.Calendars.DeleteCalendar;
public record DeleteCalendarCommand(Guid UserId, Guid LoginProviderId) : IRequest;