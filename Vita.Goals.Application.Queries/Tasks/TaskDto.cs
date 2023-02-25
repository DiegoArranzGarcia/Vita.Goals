using System;

namespace Vita.Goals.Application.Queries.Tasks;

public record TaskDto(Guid TaskId, string Title, DateTimeOffset? PlannedDateStart, DateTimeOffset? PlannedDateEnd, string Status);