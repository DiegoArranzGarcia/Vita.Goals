namespace Vita.Goals.Api.Endpoints.Goals.Get;
public record GetGoalsRequest(DateTimeOffset? StartDate = null, DateTimeOffset? EndDate = null);
