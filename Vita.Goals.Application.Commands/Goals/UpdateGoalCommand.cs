using MediatR;
using System;

namespace Vita.Goals.Application.Commands.Goals;

public record UpdateGoalCommand(Guid Id, string Title, string Description, DateTimeOffset? AimDateStart, DateTimeOffset? AimDateEnd) : IRequest;