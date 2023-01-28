using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vita.Goals.Application.Queries.Tasks
{
    public interface ITaskQueryStore
    {
        public Task<TaskDto> GetTaskById(Guid id);
        public Task<IEnumerable<TaskDto>> GetTasksCreatedByUser(Guid userId, string status = null, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null);
    }
}
