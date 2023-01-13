using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vita.Goals.Application.Queries.Tasks;
using Vita.Goals.Infrastructure.Sql.QueryStores.Configuration;

namespace Vita.Goals.Infrastructure.Sql.QueryStores
{
    public class TaskQueryStore : ITaskQueryStore
    {
        private readonly IConnectionStringProvider _connectionStringProvider;

        public TaskQueryStore(IConnectionStringProvider connectionStringProvider)
        {
            _connectionStringProvider = connectionStringProvider ?? throw new ArgumentNullException(nameof(connectionStringProvider));
        }

        public Task<TaskDto> GetTaskById(Guid id)
        {
            return Task.FromResult(new TaskDto() { Id = id });
        }
    }
}
