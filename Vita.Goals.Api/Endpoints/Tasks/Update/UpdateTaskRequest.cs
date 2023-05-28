namespace Vita.Goals.Api.Endpoints.Tasks.Update;

public record UpdateTaskRequest(string Title, Guid GoalId, DateTimeOffset? PlannedDateStart, DateTimeOffset? PlannedDateEnd);
