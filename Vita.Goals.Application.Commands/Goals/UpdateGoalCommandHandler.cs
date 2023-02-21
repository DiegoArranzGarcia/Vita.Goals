using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Vita.Goals.Domain.Aggregates.Goals;
using Vita.Goals.Domain.ValueObjects;
using Task = System.Threading.Tasks.Task;

namespace Vita.Goals.Application.Commands.Goals;

public class UpdateGoalCommandHandler : AsyncRequestHandler<UpdateGoalCommand>
{
    private readonly IGoalsRepository _goalsRepository;

    public UpdateGoalCommandHandler(IGoalsRepository goalsRepository)
    {
        _goalsRepository = goalsRepository;
    }

    protected override async Task Handle(UpdateGoalCommand request, CancellationToken cancellationToken)
    {
        Goal goal = await _goalsRepository.FindById(request.Id, cancellationToken);

        if (goal == null)
            throw new Exception("The goal wasn't found");

        goal.Title = request.Title;
        goal.Description = request.Description;
        goal.AimDate = request.AimDateStart.HasValue && request.AimDateEnd.HasValue ?
                       new DateTimeInterval(request.AimDateStart.Value, request.AimDateEnd.Value) :
                       null;

        await _goalsRepository.Update(goal);
        await _goalsRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
