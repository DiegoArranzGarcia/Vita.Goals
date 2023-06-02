using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Vita.Goals.Domain.Aggregates.Goals;

namespace Vita.Goals.Application.Commands.Goals.Create;

public class CreateGoalCommandHandler : IRequestHandler<CreateGoalCommand, Guid>
{
    private readonly IGoalRepository _goalsRepository;

    public CreateGoalCommandHandler(IGoalRepository goalsRepository)
    {
        _goalsRepository = goalsRepository;
    }

    public async Task<Guid> Handle(CreateGoalCommand request, CancellationToken cancellationToken)
    {
        Goal goal = new(request.Title, request.CreatedBy, request.Description, request.AimDate);

        await _goalsRepository.Add(goal);
        await _goalsRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return goal.Id;
    }
}
