using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Vita.Goals.Domain.Aggregates.Goals;

namespace Vita.Goals.Application.Commands.Goals;

public class CompleteGoalCommandHandler : IRequestHandler<CompleteGoalCommand>
{
    private readonly IGoalsRepository _goalsRepository;

    public CompleteGoalCommandHandler(IGoalsRepository goalsRepository)
    {
        _goalsRepository = goalsRepository;
    }

    public async Task Handle(CompleteGoalCommand request, CancellationToken cancellationToken)
    {
        Goal goal = await _goalsRepository.FindById(request.Id, cancellationToken);

        if (goal is null)
            throw new Exception("The goal wasn't found");

        goal.Complete();

        await _goalsRepository.Update(goal);
        await _goalsRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
