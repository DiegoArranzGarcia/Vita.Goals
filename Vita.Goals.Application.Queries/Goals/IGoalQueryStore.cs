using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Vita.Goals.Application.Queries.Goals;

public interface IGoalQueryStore
{
    public Task<GoalDto> GetGoalById(Guid userId, Guid id, CancellationToken cancellationToken = default);
    public Task<IEnumerable<GoalTaskDto>> GetGoalTasks(Guid userId, Guid id, CancellationToken cancellationToken = default);
    public Task<IEnumerable<GoalDto>> GetGoals(Guid userId,
                                               DateTimeOffset? startDate = null,
                                               DateTimeOffset? endDate = null,
                                               CancellationToken cancellationToken = default);
}
