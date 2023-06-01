namespace Vita.Goals.Api.Endpoints.Goals.Update;

public record UpdateGoalRequest(string Title, string Description, DateTimeOffset? AimDateStart = null, DateTimeOffset? AimDateEnd = null);