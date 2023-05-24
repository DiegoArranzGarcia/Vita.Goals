using MediatR;
using System;
using Vita.Goals.Application.Commands.Shared;

namespace Vita.Goals.Application.Commands.Goals;

public record UpdateGoalCommand(Guid Id, string Title, string Description, DateTimeOffset? AimDateStart, DateTimeOffset? AimDateEnd, User User) : IRequest;