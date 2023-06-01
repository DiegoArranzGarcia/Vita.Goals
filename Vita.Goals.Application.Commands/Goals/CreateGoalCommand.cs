using MediatR;
using System;
using Vita.Common;

namespace Vita.Goals.Application.Commands.Goals;

public record CreateGoalCommand(string Title, string Description, Guid CreatedBy, DateTimeInterval? AimDate) : IRequest<Guid>;
