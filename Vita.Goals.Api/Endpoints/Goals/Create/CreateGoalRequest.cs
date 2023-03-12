namespace Vita.Goals.Api.Endpoints.Goals.Create;

public record CreateGoalRequest(string Title, string Description, DateTimeOffset? AimDateStart, DateTimeOffset? AimDateEnd);