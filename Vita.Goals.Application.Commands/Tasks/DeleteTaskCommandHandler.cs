using MediatR;
using System.Threading;
using Vita.Goals.Domain.Aggregates.Tasks;
using Task = System.Threading.Tasks.Task;

namespace Vita.Goals.Application.Commands.Tasks;

public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand>
{
    private readonly ITaskRepository _taskRepository;

    public DeleteTaskCommandHandler(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        await _taskRepository.Delete(request.Id);
        await _taskRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
