namespace Vita.Goals.Api.Endpoints.Goals.Update;

public record UpdateGoalRequest(string Title, string Description, DateTimeOffset? AimDateStart, DateTimeOffset? AimDateEnd);