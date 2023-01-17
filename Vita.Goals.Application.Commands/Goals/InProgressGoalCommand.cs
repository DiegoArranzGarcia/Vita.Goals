using MediatR;
using System;

namespace Vita.Goals.Application.Commands.Goals
{
    public record InProgressGoalCommand : IRequest
    {
        public Guid Id { get; set; }
    }
}
