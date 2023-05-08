using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Vita.Goals.Application.Queries.Goals;
using Vita.Goals.Domain.Aggregates.Goals;
using Vita.Goals.Infrastructure.Sql.QueryStores.Configuration;

namespace Vita.Goals.Infrastructure.Sql.QueryStores;

public class GoalQueryStore : IGoalQueryStore
{
    private readonly IConnectionStringProvider _connectionStringProvider;

    public GoalQueryStore(IConnectionStringProvider connectionStringProvider)
    {
        _connectionStringProvider = connectionStringProvider ?? throw new ArgumentNullException(nameof(connectionStringProvider));
    }

    public async Task<GoalDto> GetGoalById(Guid id, CancellationToken cancellationToken = default)
    {
        using var connection = new SqlConnection(_connectionStringProvider.ConnectionString);
        await connection.OpenAsync(cancellationToken);

        const string query = @"select g.Id, g.Title, g.Description, g.AimDate_Start as AimDateStart, g.AimDate_End as AimDateEnd, gs.Name as Status, g.CreatedOn
                                 from Goals g 
                           inner join GoalStatus gs on g.GoalStatusId = gs.Id
                                where g.Id = @Id";

        CommandDefinition commandDefinition = new(commandText: query, parameters: new { id }, cancellationToken: cancellationToken);

        return await connection.QueryFirstAsync<GoalDto>(commandDefinition);
    }

    public async Task<IEnumerable<GoalTaskDto>> GetGoalTasks(Guid id, CancellationToken cancellationToken = default)
    {
        using var connection = new SqlConnection(_connectionStringProvider.ConnectionString);
        connection.Open();

        const string query = @"select t.Id as TaskId, t.Title, t.PlannedDate_Start as PlannedDateStart, t.PlannedDate_End as PlannedDateEnd, ts.Name as Status
                                 from Tasks t 
                           inner join TaskStatus ts on t.TaskStatusId = ts.Id
                                where t.AssociatedToId = @Id";

        CommandDefinition commandDefinition = new(commandText: query, parameters: new { id }, cancellationToken: cancellationToken);

        return await connection.QueryAsync<GoalTaskDto>(commandDefinition);
    }

    public async Task<IEnumerable<GoalDto>> GetGoalsCreatedByUser(Guid userId,
                                                                  DateTimeOffset? startDate = null,
                                                                  DateTimeOffset? endDate = null,
                                                                  CancellationToken cancellationToken = default)
    {
        using var connection = new SqlConnection(_connectionStringProvider.ConnectionString);
        connection.Open();

        string query = @"select g.Id, g.Title, g.Description, g.AimDate_Start as AimDateStart, g.AimDate_End as AimDateEnd, gs.Name as Status, g.CreatedOn
                           from Goals g
                     inner join GoalStatus gs on g.GoalStatusId = gs.Id
                          where CreatedBy = @UserId";


        if (startDate.HasValue && endDate.HasValue)
            query += $@" and (@Start <= g.AimDate_End and g.AimDate_Start <= @End)";

        CommandDefinition commandDefinition = new(commandText: query,
                                                  parameters: new { UserId = userId, Start = startDate, End = endDate },
                                                  cancellationToken: cancellationToken);

        return await connection.QueryAsync<GoalDto>(commandDefinition);
    }
}
