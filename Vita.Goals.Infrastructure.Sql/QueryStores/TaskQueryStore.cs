using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vita.Core.Domain;
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

        public async Task<TaskDto> GetTaskById(Guid id)
        {
            const string GetTaskByIdQuery = @"select t.Id as TaskId, t.Title, t.PlannedDate_Start as PlannedDateStart, t.PlannedDate_End as PlannedDateEnd, ts.Name as Status
                                                from Tasks t
                                                join TaskStatus ts on t.TaskStatusId = ts.Id
                                               where t.Id = @Id";

            using var connection = new SqlConnection(_connectionStringProvider.ConnectionString);
            connection.Open();

            return await connection.QueryFirstAsync<TaskDto>(GetTaskByIdQuery, param: new { Id = id });
        }

        public async Task<IEnumerable<TaskDto>> GetTasksCreatedByUser(Guid userId, string status = null, DateTimeOffset? startDate = null, DateTimeOffset? endDate  = null)
        {
            const string GetTasksCreatedByUserQuery = @"select t.Id as TaskId, t.Title, t.PlannedDate_Start as PlannedDateStart, t.PlannedDate_End as PlannedDateEnd, ts.Name as Status
                                                          from Tasks t
                                                    inner join TaskStatus ts on t.TaskStatusId = ts.Id
                                                    inner join Goals g on t.AssociatedToId = g.Id
                                                         where g.CreatedBy = @UserId";

            using var connection = new SqlConnection(_connectionStringProvider.ConnectionString);
            connection.Open();

            var sqlQuery = GetTasksCreatedByUserQuery;

            if (startDate.HasValue && endDate.HasValue)
                sqlQuery += $@" and (@Start <= t.PlannedDate_End and t.PlannedDate_Start <= @End)";

            if (!string.IsNullOrEmpty(status))
            {
                Domain.Aggregates.Tasks.TaskStatus taskStatus = Domain.Aggregates.Tasks.TaskStatus.GetAllValues().Single(x => x.Name == status);
                sqlQuery += $@" and (t.TaskStatusId != {taskStatus.Id})";
            }

            return await connection.QueryAsync<TaskDto>(sqlQuery, new { UserId = userId, Start = startDate, End = endDate });
        }
    }
}
