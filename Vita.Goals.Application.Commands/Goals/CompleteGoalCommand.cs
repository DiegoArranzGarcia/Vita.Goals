using MediatR;

namespace Vita.Goals.Application.Commands.Goals
{
    public record CompleteGoalCommand : IRequest
    {
        public Guid Id { get; set; }
    }
}
