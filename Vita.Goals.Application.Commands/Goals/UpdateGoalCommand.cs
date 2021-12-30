using MediatR;

namespace Vita.Goals.Application.Commands.Goals
{
    public record UpdateGoalCommand : IRequest
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTimeOffset? AimDateStart { get; set; }
        public DateTimeOffset? AimDateEnd { get; set; }
    }
}
