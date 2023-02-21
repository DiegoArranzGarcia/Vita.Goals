using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Vita.Goals.Application.Queries.Goals;

public interface IGoalQueryStore
{
    public Task<GoalDto> GetGoalById(Guid id, CancellationToken cancellationToken = default);
    public Task<IEnumerable<GoalTaskDto>> GetGoalTasks(Guid id, CancellationToken cancellationToken = default);
    public Task<IEnumerable<GoalDto>> GetGoalsCreatedByUser(Guid userId,
                                                            bool? showCompleted,
                                                            DateTimeOffset? startDate,
                                                            DateTimeOffset? endDate,
                                                            CancellationToken cancellationToken = default);
}
