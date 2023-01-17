using MediatR;
using System;
using Vita.Goals.Domain.ValueObjects;

namespace Vita.Goals.Application.Commands.Goals
{
    public record CreateGoalCommand : IRequest<Guid>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTimeOffset? AimDateStart { get; set; }
        public DateTimeOffset? AimDateEnd { get; set; }
    }
}
