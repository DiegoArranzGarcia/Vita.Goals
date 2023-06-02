using MediatR;
using System;
using System.Threading;
using Vita.Goals.Domain.Aggregates.Goals;
using Task = System.Threading.Tasks.Task;

namespace Vita.Goals.Application.Commands.Goals.Delete;

public class DeleteGoalCommandHandler : IRequestHandler<DeleteGoalCommand>
{
    private readonly IGoalRepository _goalsRepository;

    public DeleteGoalCommandHandler(IGoalRepository goalsRepository)
    {
        _goalsRepository = goalsRepository;
    }

    public async Task Handle(DeleteGoalCommand request, CancellationToken cancellationToken)
    {
        if (goal.CreatedBy != request.User.Id)
            throw new UnauthorizedAccessException("The goal doesn't belong to the user");

        await _goalsRepository.Delete(request.Id);
        await _goalsRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
