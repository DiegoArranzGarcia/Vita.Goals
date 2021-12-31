using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Vita.Goals.Domain.Aggregates.Goals;

namespace Vita.Goals.Application.Commands.Goals
{
    public class CompleteGoalCommandHandler : AsyncRequestHandler<CompleteGoalCommand>
    {
        private readonly IGoalsRepository _goalsRepository;

        public CompleteGoalCommandHandler(IGoalsRepository goalsRepository)
        {
            _goalsRepository = goalsRepository;
        }

        protected override async Task Handle(CompleteGoalCommand command, CancellationToken cancellationToken)
        {
            var goal = await _goalsRepository.FindByIdAsync(command.Id);

            if (goal == null)
                throw new Exception("The goal wasn't found");

            goal.Complete();

            await _goalsRepository.Update(goal);
            await _goalsRepository.UnitOfWork.SaveEntitiesAsync();
        }
    }
}
