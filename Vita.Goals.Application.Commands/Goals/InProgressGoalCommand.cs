using MediatR;
using System;

namespace Vita.Goals.Application.Commands.Goals;

public record InProgressGoalCommand(Guid Id) : IRequest;