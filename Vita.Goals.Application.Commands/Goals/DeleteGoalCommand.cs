using MediatR;
using System;

namespace Vita.Goals.Application.Commands.Goals
{
    public record DeleteGoalCommand : IRequest
    {
        public Guid Id { get; set; }
    }
}
