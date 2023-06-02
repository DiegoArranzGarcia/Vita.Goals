using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Vita.Goals.Domain.Aggregates.Goals;

namespace Vita.Goals.Application.Commands.Goals.Ready;

public class ReadyGoalCommandHandler : IRequestHandler<ReadyGoalCommand>
{
    private readonly IGoalRepository _goalsRepository;

    public ReadyGoalCommandHandler(IGoalRepository goalsRepository)
    {
        _goalsRepository = goalsRepository;
    }

    public async Task Handle(ReadyGoalCommand request, CancellationToken cancellationToken)
    {
        Goal goal = await _goalsRepository.FindById(request.Id, cancellationToken);

        if (goal.CreatedBy != request.User.Id)
            throw new UnauthorizedAccessException("The goal doesn't belong to the user");

        goal.Ready();

        await _goalsRepository.Update(goal);
        await _goalsRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
