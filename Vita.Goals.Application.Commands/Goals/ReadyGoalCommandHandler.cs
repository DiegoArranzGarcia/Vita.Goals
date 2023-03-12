using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Vita.Goals.Domain.Aggregates.Goals;
using Vita.Goals.Domain.ValueObjects;

namespace Vita.Goals.Application.Commands.Goals;

public class ReadyGoalCommandHandler : IRequestHandler<ReadyGoalCommand>
{
    private readonly IGoalsRepository _goalsRepository;

    public ReadyGoalCommandHandler(IGoalsRepository goalsRepository)
    {
        _goalsRepository = goalsRepository;
    }

    public async Task Handle(ReadyGoalCommand request, CancellationToken cancellationToken)
    {
        Goal goal = await _goalsRepository.FindById(request.Id, cancellationToken);

        if (goal == null)
            throw new Exception("The goal wasn't found");

        goal.Ready();

        await _goalsRepository.Update(goal);
        await _goalsRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
