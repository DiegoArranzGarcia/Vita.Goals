using MediatR;
using System;

namespace Vita.Goals.Application.Commands.Goals;

public record DeleteTaskCommand(Guid Id) : IRequest;