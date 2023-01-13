using MediatR;
using System;

namespace Vita.Goals.Application.Commands.Goals
{
    public record DeleteTaskCommand : IRequest
    {
        public Guid Id { get; set; }
    }
}
