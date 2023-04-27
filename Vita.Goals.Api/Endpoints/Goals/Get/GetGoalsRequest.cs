namespace Vita.Goals.Api.Endpoints.Goals.Get;
public record GetGoalsRequest(bool? ShowCompleted = null, DateTimeOffset? StartDate = null, DateTimeOffset? EndDate = null);
