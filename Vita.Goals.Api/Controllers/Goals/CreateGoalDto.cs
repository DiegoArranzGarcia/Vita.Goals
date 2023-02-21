namespace Vita.Goals.Api.Controllers.Goals;

public record CreateGoalDto(string Title, string Description, DateTimeOffset? AimDateStart, DateTimeOffset? AimDateEnd);