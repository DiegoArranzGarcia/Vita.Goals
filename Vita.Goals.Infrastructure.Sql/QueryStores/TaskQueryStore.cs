using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vita.Goals.Application.Queries.Goals;
using Vita.Goals.Application.Queries.Tasks;
using Vita.Goals.Domain.Aggregates.Goals;
using Vita.Goals.Infrastructure.Sql.QueryStores.Configuration;

namespace Vita.Goals.Infrastructure.Sql.QueryStores
{
    public class TaskQueryStore : ITaskQueryStore
    {
        public const string GetTaskByIdQuery = @"select t.Id as TaskId, t.Title, t.PlannedDate_Start as PlannedDateStart, t.PlannedDate_End as PlannedDateEnd, ts.Name as Status
                                                 from Tasks t
                                                 join TaskStatus ts on t.TaskStatusId = ts.Id
                                                 where t.Id = @Id";

        private const string GetTasksCreatedByUserQuery = @"select t.Id as TaskId, t.Title, t.PlannedDate_Start as PlannedDateStart, t.PlannedDate_End as PlannedDateEnd, ts.Name as Status
                                                              from Tasks t
                                                        inner join TaskStatus ts on t.TaskStatusId = ts.Id
                                                        inner join Goals g on t.AssociatedToId = g.Id
                                                             where g.CreatedBy = @UserId";

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

        public async Task<IEnumerable<TaskDto>> GetTasksCreatedByUser(Guid userId, bool? showCompleted, DateTimeOffset? startDate, DateTimeOffset? endDate)
        {
            using var connection = new SqlConnection(_connectionStringProvider.ConnectionString);
            connection.Open();

            var sqlQuery = GetTasksCreatedByUserQuery;

            if (startDate.HasValue && endDate.HasValue)
                sqlQuery += $@" and (@Start <= t.PlannedDate_End and t.PlannedDate_Start <= @End)";

            if (!showCompleted.HasValue || !showCompleted.Value)
                sqlQuery += $@" and (t.TaskStatusId != {GoalStatus.Completed.Id})";

            return await connection.QueryAsync<TaskDto>(sqlQuery, new { UserId = userId, Start = startDate, End = endDate });
        }
    }
}
