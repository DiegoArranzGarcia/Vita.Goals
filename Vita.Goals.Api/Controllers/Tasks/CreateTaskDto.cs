namespace Vita.Goals.Api.Controllers.Tasks
{
    public record CreateTaskDto
    {
        public Guid? GoalId { get; init; }
        public string Title { get; init; }
        public DateTimeOffset? PlannedDateStart { get; init; }
        public DateTimeOffset? PlannedDateEnd { get; init; }
    }
}
