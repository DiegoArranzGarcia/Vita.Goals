using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Vita.Goals.Domain.Aggregates.Goals;

namespace Vita.Goals.Application.Commands.Goals;

public class InProgressGoalCommandHandler : IRequestHandler<InProgressGoalCommand>
{
    private readonly IGoalsRepository _goalsRepository;

    public InProgressGoalCommandHandler(IGoalsRepository goalsRepository)
    {
        _goalsRepository = goalsRepository;
    }

    public async Task Handle(InProgressGoalCommand request, CancellationToken cancellationToken)
    {
        Goal goal = await _goalsRepository.FindById(request.Id, cancellationToken) ?? throw new Exception("The goal wasn't found");

        if (goal.CreatedBy != request.User.Id)
            throw new UnauthorizedAccessException("The goal doesn't belong to the user");

        goal.InProgress();

        await _goalsRepository.Update(goal);
        await _goalsRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
