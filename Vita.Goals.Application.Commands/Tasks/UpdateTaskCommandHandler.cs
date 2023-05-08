using MediatR;
using System;
using System.Threading;
using Vita.Goals.Domain.Aggregates.Goals;
using Vita.Goals.Domain.Aggregates.Tasks;
using Vita.Goals.Domain.ValueObjects;

namespace Vita.Goals.Application.Commands.Tasks;

public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand>
{
    private readonly IGoalsRepository _goalsRepository;
    private readonly ITaskRepository _taskRepository;

    public UpdateTaskCommandHandler(IGoalsRepository goalsRepository, ITaskRepository taskRepository)
    {
        _goalsRepository = goalsRepository;
        _taskRepository = taskRepository;
    }

    public async System.Threading.Tasks.Task Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        Task task = await _taskRepository.FindById(request.TaskId, cancellationToken);

        if (task == null)
            throw new Exception("The task doesn't exist");

        Goal goal = request.GoalId.HasValue ? await _goalsRepository.FindById(request.GoalId.Value, cancellationToken) : null;

        task.AssociatedTo = goal;
        task.Title = request.Title;
        task.PlannedDate = request.PlannedDateStart.HasValue && request.PlannedDateEnd.HasValue ?
                           new DateTimeInterval(request.PlannedDateStart.Value, request.PlannedDateEnd.Value) :
                           null;

        await _taskRepository.Update(task);
        await _taskRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
