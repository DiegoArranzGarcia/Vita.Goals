using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vita.Core.Domain;
using Vita.Goals.Application.Queries.Tasks;
using Vita.Goals.Infrastructure.Sql.QueryStores.Configuration;

namespace Vita.Goals.Infrastructure.Sql.QueryStores;

public class TaskQueryStore : ITaskQueryStore
{
    private readonly IConnectionStringProvider _connectionStringProvider;

    public TaskQueryStore(IConnectionStringProvider connectionStringProvider)
    {
        _connectionStringProvider = connectionStringProvider ?? throw new ArgumentNullException(nameof(connectionStringProvider));
    }

    public async Task<TaskDto> GetTaskById(Guid userId, Guid id, CancellationToken cancellationToken = default)
    {
        using var connection = new SqlConnection(_connectionStringProvider.ConnectionString);
        connection.Open();

        await CheckUserHasAccessToGoal(userId, id, connection, cancellationToken);

        const string getTaskByIdQuery = @"select t.Id, t.Title, t.PlannedDate_Start as PlannedDateStart, t.PlannedDate_End as PlannedDateEnd, ts.Name as Status
                                            from Tasks t
                                            join TaskStatus ts on t.TaskStatusId = ts.Id
                                           where t.Id = @Id";

        CommandDefinition commandDefinition = new(getTaskByIdQuery, parameters: new { Id = id }, cancellationToken: cancellationToken);

        return await connection.QueryFirstAsync<TaskDto>(commandDefinition);
    }

    public async Task<IEnumerable<TaskDto>> GetTasksCreatedByUser(Guid userId,
                                                                  string? status = null,
                                                                  DateTimeOffset? startDate = null,
                                                                  DateTimeOffset? endDate = null,
                                                                  CancellationToken cancellationToken = default)
    {
        const string getTasksCreatedByUserQuery = @"select t.Id, t.Title, t.PlannedDate_Start as PlannedDateStart, t.PlannedDate_End as PlannedDateEnd, ts.Name as Status
                                                      from Tasks t
                                                inner join TaskStatus ts on t.TaskStatusId = ts.Id
                                                 left join Goals g on t.AssociatedToId = g.Id
                                                     where g.CreatedBy = @UserId";

        using var connection = new SqlConnection(_connectionStringProvider.ConnectionString);
        connection.Open();

        var sqlQuery = getTasksCreatedByUserQuery;

        if (startDate.HasValue && endDate.HasValue)
            sqlQuery += $@" and (@Start <= t.PlannedDate_End and t.PlannedDate_Start <= @End)";

        if (!string.IsNullOrEmpty(status))
        {
            Domain.Aggregates.Tasks.TaskStatus taskStatus = Enumeration.FromDisplayName<Domain.Aggregates.Tasks.TaskStatus>(status);
            sqlQuery += $@" and (t.TaskStatusId != {taskStatus.Id})";
        }

        CommandDefinition commandDefinition = new(sqlQuery, parameters: new { UserId = userId, Start = startDate, End = endDate }, cancellationToken: cancellationToken);

        return await connection.QueryAsync<TaskDto>(commandDefinition);
    }

    private static async Task CheckUserHasAccessToGoal(Guid userId, Guid id, SqlConnection connection, CancellationToken cancellationToken)
    {
        const string authorizationQuery = @"select g.CreatedBy                 
                                              from Tasks t
                                              join Goals g on t.AssociatedToId = g.Id
                                             where t.Id = @Id";

        CommandDefinition commandDefinition = new(authorizationQuery, parameters: new { id }, cancellationToken: cancellationToken);
        dynamic result = await connection.QueryFirstOrDefaultAsync<dynamic>(commandDefinition) ??
                         throw new KeyNotFoundException();

        Guid createdBy = (Guid)result.CreatedBy;
        if (createdBy != userId)
            throw new UnauthorizedAccessException();
    }

}
