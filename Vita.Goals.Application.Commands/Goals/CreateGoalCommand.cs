using MediatR;
using System;

namespace Vita.Goals.Application.Commands.Goals;

public record CreateGoalCommand(string Title, string Description, Guid CreatedBy, DateTimeOffset? AimDateStart, DateTimeOffset? AimDateEnd) : IRequest<Guid>;
