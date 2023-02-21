using System;

namespace Vita.Goals.Application.Queries.Goals;

public record GoalDto(Guid Id, string Title, string Description, DateTimeOffset? AimDateStart, DateTimeOffset? AimDateEnd, string Status, DateTimeOffset CreatedOn);

public record GoalTaskDto(Guid TaskId, string Title, DateTimeOffset? PlannedDateStart, DateTimeOffset? PlannedDateEnd, string Status);