namespace Vita.Goals.Api.Controllers.Goals;

public record UpdateGoalDto(string Title, string Description, DateTimeOffset? AimDateStart, DateTimeOffset? AimDateEnd);