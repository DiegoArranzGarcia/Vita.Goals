using MediatR;
using System;
using Vita.Goals.Domain.ValueObjects;

namespace Vita.Goals.Application.Commands.Tasks
{
    public record UpdateTaskCommand : IRequest
    {
        public Guid TaskId { get; set; }
        public string Title { get; set; }
        public Guid? GoalId { get; set; }
        public DateTimeOffset? PlannedDateStart { get; set; }
        public DateTimeOffset? PlannedDateEnd { get; set; }
    }
}
