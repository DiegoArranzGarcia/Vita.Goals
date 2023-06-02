using MediatR;
using System;

namespace Vita.Goals.Application.Commands.Tasks.Delete;

public record DeleteTaskCommand(Guid Id) : IRequest;