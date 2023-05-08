using MediatR;
using System;

namespace Vita.Goals.Application.Commands.Goals;

public record ReadyGoalCommand(Guid Id) : IRequest;
