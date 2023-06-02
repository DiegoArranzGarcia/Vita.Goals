using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Vita.Goals.Domain.Aggregates.Goals;

namespace Vita.Goals.Application.Commands.Goals.Complete;

public class CompleteGoalCommandHandler : IRequestHandler<CompleteGoalCommand>
{
    private readonly IGoalRepository _goalsRepository;

    public CompleteGoalCommandHandler(IGoalRepository goalsRepository)
    {
        _goalsRepository = goalsRepository;
    }

    public async Task Handle(CompleteGoalCommand request, CancellationToken cancellationToken)
    {
        Goal goal = await _goalsRepository.FindById(request.Id, cancellationToken);

        if (goal.CreatedBy != request.User.Id)
            throw new UnauthorizedAccessException("The goal doesn't belong to the user");

        goal.Complete();

        await _goalsRepository.Update(goal);
        await _goalsRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
