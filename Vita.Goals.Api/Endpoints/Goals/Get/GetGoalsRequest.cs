namespace Vita.Goals.Api.Endpoints.Goals.GetGoals;
public record GetGoalsRequest(bool? ShowCompleted = null, DateTimeOffset? StartDate = null, DateTimeOffset? EndDate = null);
