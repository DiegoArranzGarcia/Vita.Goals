using MediatR;
using System;

namespace Vita.Goals.Application.Commands.Tasks;

public record CreateTaskCommand(Guid? GoalId, string Title, DateTimeOffset? PlannedDateStart, DateTimeOffset? PlannedDateEnd) : IRequest<Guid>;
