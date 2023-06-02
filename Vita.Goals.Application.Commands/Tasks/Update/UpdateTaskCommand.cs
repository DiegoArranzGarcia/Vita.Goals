using MediatR;
using System;
using Vita.Goals.Application.Commands.Shared;

namespace Vita.Goals.Application.Commands.Tasks.Update;

public record UpdateTaskCommand(Guid Id, string Title, Guid GoalId, User User, DateTimeOffset? PlannedDateStart, DateTimeOffset? PlannedDateEnd) : IRequest;
