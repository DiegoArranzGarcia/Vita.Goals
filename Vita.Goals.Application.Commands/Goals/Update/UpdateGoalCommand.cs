using MediatR;
using System;
using Vita.Common;
using Vita.Goals.Application.Commands.Shared;
using Vita.Goals.Domain.ValueObjects;

namespace Vita.Goals.Application.Commands.Goals.Update;

public record UpdateGoalCommand(Guid Id, string Title, string Description, DateTimeInterval? AimDate, User User) : IRequest;