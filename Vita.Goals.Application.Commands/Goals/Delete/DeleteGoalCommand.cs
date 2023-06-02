using MediatR;
using System;
using Vita.Goals.Application.Commands.Shared;

namespace Vita.Goals.Application.Commands.Goals.Delete;

public record DeleteGoalCommand(Guid Id, User User) : IRequest;
