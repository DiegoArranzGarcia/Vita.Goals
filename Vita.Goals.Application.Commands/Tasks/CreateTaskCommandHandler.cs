using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Vita.Common;
using Vita.Goals.Domain.Aggregates.Goals;
using Vita.Goals.Domain.Aggregates.Tasks;
using Vita.Goals.Domain.ValueObjects;

namespace Vita.Goals.Application.Commands.Tasks;

internal class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, Guid>
{
    private readonly IGoalsRepository _goalRepository;
    private readonly ITaskRepository _taskRepository;

    public CreateTaskCommandHandler(ITaskRepository taskRepository, IGoalsRepository goalRepository)
    {
        _taskRepository = taskRepository;
        _goalRepository = goalRepository;
    }

    public async Task<Guid> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        Goal goal = await _goalRepository.FindById(request.GoalId, cancellationToken) ?? throw new KeyNotFoundException();

        if (goal.CreatedBy != request.User.Id)
            throw new UnauthorizedAccessException();

        DateTimeInterval? plannedDate = request.PlannedDateStart.HasValue && request.PlannedDateEnd.HasValue ?
                                        new DateTimeInterval(request.PlannedDateStart.Value, request.PlannedDateEnd.Value) :
                                        null;

        Domain.Aggregates.Tasks.Task task = new(request.Title, goal, plannedDate);

        await _taskRepository.Add(task);
        await _taskRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return task.Id;
    }
}
