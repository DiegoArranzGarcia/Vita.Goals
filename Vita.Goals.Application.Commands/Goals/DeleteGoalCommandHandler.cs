using MediatR;
using Vita.Goals.Domain.Aggregates.Goals;

namespace Vita.Goals.Application.Commands.Goals
{
    public class DeleteGoalCommandHandler : AsyncRequestHandler<DeleteGoalCommand>
    {
        private readonly IGoalsRepository _goalsRepository;

        public DeleteGoalCommandHandler(IGoalsRepository goalsRepository)
        {
            _goalsRepository = goalsRepository;
        }

        protected override async Task Handle(DeleteGoalCommand request, CancellationToken cancellationToken)
        {
            await _goalsRepository.Delete(request.Id);
            await _goalsRepository.UnitOfWork.SaveEntitiesAsync();
        }
    }
}
