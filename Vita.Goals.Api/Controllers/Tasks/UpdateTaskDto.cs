namespace Vita.Goals.Api.Controllers.Tasks
{
    public record UpdateTaskDto
    {
        public string Title { get; init; }
        public Guid? GoalId { get; init; }
        public DateTimeOffset? PlannedDateStart { get; init; }
        public DateTimeOffset? PlannedDateEnd { get; init; }
    }
}
