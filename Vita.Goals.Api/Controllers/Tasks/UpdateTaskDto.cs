namespace Vita.Goals.Api.Controllers.Tasks;

public record UpdateTaskDto(string Title, Guid? GoalId, DateTimeOffset? PlannedDateStart, DateTimeOffset? PlannedDateEnd);
