using MediatR;
using System;

namespace Vita.Goals.Application.Commands.Goals;

public record DeleteGoalCommand(Guid Id) : IRequest;    
