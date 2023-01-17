using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;
using Vita.Goals.Application.Queries.Tasks;
using Vita.Goals.Infrastructure.Sql.QueryStores.Configuration;

namespace Vita.Goals.Infrastructure.Sql.QueryStores
{
    public class TaskQueryStore : ITaskQueryStore
    {
        public const string GetTaskByIdQuery = @"select t.Id as TaskId, t.Title, t.PlannedDate_Start as PlannedDateStart, t.PlannedDate_End as PlannedDateEnd, ts.Name as Status
                                                 from Tasks t
                                                 join TaskStatus ts on t.TaskStatusId = ts.Id
                                                 where t.Id = @Id";


        private readonly IConnectionStringProvider _connectionStringProvider;

        public TaskQueryStore(IConnectionStringProvider connectionStringProvider)
        {
            _connectionStringProvider = connectionStringProvider ?? throw new ArgumentNullException(nameof(connectionStringProvider));
        }

        public async Task<TaskDto> GetTaskById(Guid id)
        {
            using var connection = new SqlConnection(_connectionStringProvider.ConnectionString);
            connection.Open();

            return await connection.QueryFirstAsync<TaskDto>(GetTaskByIdQuery, param: new { Id = id });
        }
    }
}
