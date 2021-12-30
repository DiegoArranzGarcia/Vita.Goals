using MediatR;
using Vita.Goals.Domain.Aggregates.Goals;

namespace Vita.Goals.Application.Commands.Goals
{
    public class CreateGoalCommandHandler : IRequestHandler<CreateGoalCommand, Guid>
    {
        private readonly IGoalsRepository _goalsRepository;

        public CreateGoalCommandHandler(IGoalsRepository goalsRepository)
        {
            _goalsRepository = goalsRepository;
        }

        public async Task<Guid> Handle(CreateGoalCommand request, CancellationToken cancellationToken)
        {
            var goal = new Goal(request.Title, request.CreatedBy, request.Description, request.AimDate);

            await _goalsRepository.Add(goal);
            await _goalsRepository.UnitOfWork.SaveEntitiesAsync();

            return goal.Id;
        }
    }
}
