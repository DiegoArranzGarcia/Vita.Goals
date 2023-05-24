namespace Vita.Goals.Api.Endpoints.Tasks.Create;

public record CreateTaskRequest(string Title, Guid GoalId, DateTimeOffset? PlannedDateStart = null, DateTimeOffset? PlannedDateEnd = null);
