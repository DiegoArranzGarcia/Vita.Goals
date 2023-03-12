using System;

namespace Vita.Goals.Api.Endpoints.Calendars.Create;

public record CreateCalendarRequest(Guid UserId, string ProviderName);
