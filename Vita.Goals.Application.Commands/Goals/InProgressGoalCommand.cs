using MediatR;
using System;
using Vita.Goals.Application.Commands.Shared;

namespace Vita.Goals.Application.Commands.Goals;

public record InProgressGoalCommand(Guid Id, User User) : IRequest;