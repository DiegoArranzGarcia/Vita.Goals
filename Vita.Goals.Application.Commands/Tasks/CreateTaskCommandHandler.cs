using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Vita.Goals.Domain.Aggregates.Goals;
using Vita.Goals.Domain.Aggregates.Tasks;
using Vita.Goals.Domain.ValueObjects;

namespace Vita.Goals.Application.Commands.Tasks
{
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
            Goal goal = request.GoalId.HasValue ? await _goalRepository.FindById(request.GoalId.Value): null;

            DateTimeInterval plannedDate = request.PlannedDateStart.HasValue && request.PlannedDateEnd.HasValue ?
                                           new DateTimeInterval(request.PlannedDateStart.Value, request.PlannedDateEnd.Value) :
                                           null;

            Domain.Aggregates.Tasks.Task task = new(request.Title, plannedDate, goal);

            await _taskRepository.Add(task);
            await _taskRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            return task.Id;
        }
    }
}
