using MediatR;
using System;

namespace Vita.Goals.Application.Commands.Tasks;

public record UpdateTaskCommand(Guid TaskId, string Title, Guid? GoalId, DateTimeOffset? PlannedDateStart, DateTimeOffset? PlannedDateEnd) : IRequest;
