using MediatR;
using System;

namespace Vita.Goals.Application.Commands.Tasks;

public record DeleteTaskCommand(Guid Id) : IRequest;