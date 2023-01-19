using MediatR;
using System;

namespace Vita.Goals.Application.Commands.Goals
{
    public record ReadyGoalCommand : IRequest
    {
        public Guid Id { get; set; }
    }
}
