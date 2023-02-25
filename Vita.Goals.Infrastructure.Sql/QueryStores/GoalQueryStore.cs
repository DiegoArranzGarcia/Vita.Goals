using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vita.Goals.Application.Queries.Goals;
using Vita.Goals.Application.Queries.Tasks;
using Vita.Goals.Domain.Aggregates.Goals;
using Vita.Goals.Infrastructure.Sql.QueryStores.Configuration;

namespace Vita.Goals.Infrastructure.Sql.QueryStores;

public class GoalQueryStore : IGoalQueryStore
{
    private const string GetGoalByIdQuery = @"select g.Id, g.Title, g.Description, g.AimDate_Start as AimDateStart, g.AimDate_End as AimDateEnd, gs.Name as Status, g.CreatedOn
                                                  from Goals g 
                                                  inner join GoalStatus gs on g.GoalStatusId = gs.Id
                                                  where g.Id = @Id";

    private const string GetGoalsCreatedByUserQuery = @"select g.Id, g.Title, g.Description, g.AimDate_Start as AimDateStart, g.AimDate_End as AimDateEnd, gs.Name as Status, g.CreatedOn
                                                              from Goals g
                                                        inner join GoalStatus gs on g.GoalStatusId = gs.Id
                                                             where CreatedBy = @UserId";

    private const string GetGoalTasksQuery = @"select t.Id as TaskId, t.Title, t.PlannedDate_Start as PlannedDateStart, t.PlannedDate_End as PlannedDateEnd, ts.Name as Status
                                                   from Tasks t 
                                                   inner join TaskStatus ts on t.TaskStatusId = ts.Id
                                                   where t.AssociatedToId = @Id";


    private readonly IConnectionStringProvider _connectionStringProvider;

    public GoalQueryStore(IConnectionStringProvider connectionStringProvider)
    {
        _connectionStringProvider = connectionStringProvider ?? throw new ArgumentNullException(nameof(connectionStringProvider));
    }

    public async Task<GoalDto> GetGoalById(Guid id, CancellationToken cancellationToken = default)
    {
        using var connection = new SqlConnection(_connectionStringProvider.ConnectionString);
        await connection.OpenAsync(cancellationToken);

        CommandDefinition commandDefinition = new(commandText: GetGoalByIdQuery, parameters: new { id }, cancellationToken: cancellationToken);

        return await connection.QueryFirstAsync<GoalDto>(commandDefinition);
    }

    public async Task<IEnumerable<GoalTaskDto>> GetGoalTasks(Guid id, CancellationToken cancellationToken = default)
    {
        using var connection = new SqlConnection(_connectionStringProvider.ConnectionString);
        connection.Open();

        CommandDefinition commandDefinition = new(commandText: GetGoalTasksQuery, parameters: new { id }, cancellationToken: cancellationToken);

        return await connection.QueryAsync<GoalTaskDto>(commandDefinition);
    }

    public async Task<IEnumerable<GoalDto>> GetGoalsCreatedByUser(Guid userId,
                                                                  bool? showCompleted,
                                                                  DateTimeOffset? startDate,
                                                                  DateTimeOffset? endDate, 
                                                                  CancellationToken cancellationToken = default)
    {
        using var connection = new SqlConnection(_connectionStringProvider.ConnectionString);
        connection.Open();

        var sqlQuery = GetGoalsCreatedByUserQuery;

        if (startDate.HasValue && endDate.HasValue)
            sqlQuery += $@" and (@Start <= g.AimDate_End and g.AimDate_Start <= @End)";

        if (!showCompleted.HasValue || !showCompleted.Value)
            sqlQuery += $@" and (g.GoalStatusId = {GoalStatus.ToBeDefined.Id})";

        CommandDefinition commandDefinition = new(commandText: sqlQuery,
                                                  parameters: new { UserId = userId, Start = startDate, End = endDate },
                                                  cancellationToken: cancellationToken);

        return await connection.QueryAsync<GoalDto>(commandDefinition);
    }
}
