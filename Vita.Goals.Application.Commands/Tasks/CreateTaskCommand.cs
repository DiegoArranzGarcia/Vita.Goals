using MediatR;
using System;
using Vita.Goals.Application.Commands.Shared;

namespace Vita.Goals.Application.Commands.Tasks;

public record CreateTaskCommand(Guid GoalId, string Title, User User, DateTimeOffset? PlannedDateStart, DateTimeOffset? PlannedDateEnd) : IRequest<Guid>;
