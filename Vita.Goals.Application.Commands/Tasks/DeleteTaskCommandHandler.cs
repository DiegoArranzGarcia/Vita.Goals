using MediatR;
using System.Threading;
using Vita.Goals.Domain.Aggregates.Tasks;
using Task = System.Threading.Tasks.Task;

namespace Vita.Goals.Application.Commands.Goals;

public class DeleteTaskCommandHandler : AsyncRequestHandler<DeleteTaskCommand>
{
    private readonly ITaskRepository _taskRepository;

    public DeleteTaskCommandHandler(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    protected override async Task Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        await _taskRepository.Delete(request.Id);
        await _taskRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
