using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Vita.Goals.Domain.Aggregates.Goals;
using Vita.Goals.Domain.ValueObjects;

namespace Vita.Goals.Application.Commands.Goals
{
    public class CompleteGoalCommandHandler : AsyncRequestHandler<CompleteGoalCommand>
    {
        private readonly IGoalsRepository _goalsRepository;

        public CompleteGoalCommandHandler(IGoalsRepository goalsRepository)
        {
            _goalsRepository = goalsRepository;
        }

        protected override async Task Handle(CompleteGoalCommand request, CancellationToken cancellationToken)
        {
            Goal goal = await _goalsRepository.FindById(request.Id);

            if (goal == null)
                throw new Exception("The goal wasn't found");

            goal.Complete();

            await _goalsRepository.Update(goal);
            await _goalsRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
