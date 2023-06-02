using MediatR;
using System;
using Vita.Goals.Application.Commands.Shared;

namespace Vita.Goals.Application.Commands.Goals.Complete;

public record CompleteGoalCommand(Guid Id, User User) : IRequest;
