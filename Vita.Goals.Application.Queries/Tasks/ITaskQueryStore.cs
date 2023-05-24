using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Vita.Goals.Application.Queries.Tasks;

public interface ITaskQueryStore
{
    public Task<TaskDto> GetTaskById(Guid id, CancellationToken cancellationToken = default);
    public Task<IEnumerable<TaskDto>> GetTasksCreatedByUser(Guid userId, string? status = null, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, CancellationToken cancellationToken = default);
}
