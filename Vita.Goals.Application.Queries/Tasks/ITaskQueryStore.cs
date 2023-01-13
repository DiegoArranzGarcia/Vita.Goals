using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vita.Goals.Application.Queries.Goals;

namespace Vita.Goals.Application.Queries.Tasks
{
    public interface ITaskQueryStore
    {
        public Task<TaskDto> GetTaskById(Guid id);
    }
}
