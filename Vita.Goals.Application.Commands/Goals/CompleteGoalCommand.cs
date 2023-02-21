using MediatR;
using System;

namespace Vita.Goals.Application.Commands.Goals;

public record CompleteGoalCommand(Guid Id) : IRequest;    
