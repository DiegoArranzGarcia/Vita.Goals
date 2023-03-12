namespace Vita.Goals.Api.Endpoints.Tasks.Get;
public record GetTasksRequest(Guid? UserId = null, string? Status = null, DateTimeOffset? StartDate = null, DateTimeOffset? EndDate = null);
