namespace Vita.Goals.Api.Controllers.Tasks;

public record CreateTaskDto(string Title, Guid? GoalId, DateTimeOffset? PlannedDateStart, DateTimeOffset? PlannedDateEnd);
