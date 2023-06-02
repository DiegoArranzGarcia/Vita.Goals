using MediatR;
using System;
using Vita.Common;
using Vita.Goals.Application.Commands.Shared;

namespace Vita.Goals.Application.Commands.Tasks.Create;

public record CreateTaskCommand(Guid GoalId, string Title, User User, DateTimeInterval? PlannedDate) : IRequest<Guid>;
