using MediatR;
using System;
using Vita.Goals.Application.Commands.Shared;

namespace Vita.Goals.Application.Commands.Goals.Ready;

public record ReadyGoalCommand(Guid Id, User User) : IRequest;
