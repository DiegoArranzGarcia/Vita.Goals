namespace Vita.Goals.Api.Controllers.Goals
{
    public record CreateGoalDto
    {
        public string Title { get; init; }
        public string Description { get; init; }
        public Guid CreatedBy { get; init; }
        public DateTimeOffset? AimDateStart { get; init; }
        public DateTimeOffset? AimDateEnd { get; init; }
    }
}
