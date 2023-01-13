using MediatR;
using System;
using Vita.Goals.Domain.ValueObjects;

namespace Vita.Goals.Application.Commands.Tasks
{
    public record UpdateTaskCommand : IRequest
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public Guid? AssociatedGoalId { get; set; }
        public DateTimeInterval AimDate { get; set; }
    }
}
