using System;

namespace Vita.Goals.Application.Queries.Tasks;

public record TaskDto(Guid Id, string Title, DateTimeOffset? PlannedDateStart, DateTimeOffset? PlannedDateEnd, string Status);