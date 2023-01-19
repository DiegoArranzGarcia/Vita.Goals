using MediatR;
using System;
using Vita.Goals.Domain.ValueObjects;

namespace Vita.Goals.Application.Commands.Tasks
{
    public record CreateTaskCommand : IRequest<Guid>
    {
        public Guid? GoalId { get; init; }
        public string Title { get; init; }
        public DateTimeOffset? PlannedDateStart { get; set; }
        public DateTimeOffset? PlannedDateEnd { get; set; }
    }
}
