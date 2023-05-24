using MediatR;
using System;
using System.Threading;
using Vita.Goals.Domain.Aggregates.Goals;
using Vita.Goals.Domain.ValueObjects;
using Task = System.Threading.Tasks.Task;

namespace Vita.Goals.Application.Commands.Goals;

public class UpdateGoalCommandHandler : IRequestHandler<UpdateGoalCommand>
{
    private readonly IGoalsRepository _goalsRepository;

    public UpdateGoalCommandHandler(IGoalsRepository goalsRepository)
    {
        _goalsRepository = goalsRepository;
    }

    public async Task Handle(UpdateGoalCommand request, CancellationToken cancellationToken)
    {
        Goal goal = await _goalsRepository.FindById(request.Id, cancellationToken);

        if (goal.CreatedBy != request.User.Id)
            throw new UnauthorizedAccessException("The goal doesn't belong to the user");

        goal.Title = request.Title;
        goal.Description = request.Description;
        goal.AimDate = request.AimDateStart.HasValue && request.AimDateEnd.HasValue ?
                       new DateTimeInterval(request.AimDateStart.Value, request.AimDateEnd.Value) :
                       null;

        await _goalsRepository.Update(goal);
        await _goalsRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
