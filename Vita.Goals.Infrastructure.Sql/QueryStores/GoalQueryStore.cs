using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;
using Vita.Goals.Application.Queries.Goals;
using Vita.Goals.Infrastructure.Sql.QueryStores.Configuration;

namespace Vita.Goals.Infrastructure.Sql.QueryStores;

public class GoalQueryStore : IGoalQueryStore
{
    private readonly IConnectionStringProvider _connectionStringProvider;

    public GoalQueryStore(IConnectionStringProvider connectionStringProvider)
    {
        _connectionStringProvider = connectionStringProvider ?? throw new ArgumentNullException(nameof(connectionStringProvider));
    }

    public async Task<GoalDto> GetGoalById(Guid userId, Guid id, CancellationToken cancellationToken = default)
    {
        const string query = @"select g.Id, g.Title, g.Description, g.AimDate_Start as AimDateStart, g.AimDate_End as AimDateEnd, gs.Name as Status, g.CreatedOn, g.CreatedBy
                                 from Goals g 
                           inner join GoalStatus gs on g.GoalStatusId = gs.Id
                                where g.Id = @Id";

        using var connection = new SqlConnection(_connectionStringProvider.ConnectionString);
        await connection.OpenAsync(cancellationToken);

        CommandDefinition commandDefinition = new(query, parameters: new { id }, cancellationToken: cancellationToken);
        dynamic result = await connection.QueryFirstOrDefaultAsync<dynamic>(commandDefinition) ?? 
                         throw new KeyNotFoundException();
        
        Guid createdBy = (Guid)result.CreatedBy;
        if (createdBy != userId)
            throw new UnauthorizedAccessException();

        return new GoalDto
        (
            Id: result.Id,
            Title: result.Title,
            Description: result.Description,
            AimDateStart: result.AimDateStart,
            AimDateEnd: result.AimDateEnd,
            Status: result.Status,
            CreatedOn: result.CreatedOn
        );
    }

    public async Task<IEnumerable<GoalTaskDto>> GetGoalTasks(Guid userId, Guid id, CancellationToken cancellationToken = default)
    {
        using var connection = new SqlConnection(_connectionStringProvider.ConnectionString);
        connection.Open();

        await CheckIfUserHasAccessToGoal(userId, id, connection, cancellationToken);

        const string query = @"select t.Id as TaskId, t.Title, t.PlannedDate_Start as PlannedDateStart, t.PlannedDate_End as PlannedDateEnd, ts.Name as Status
                                 from Tasks t 
                           inner join TaskStatus ts on t.TaskStatusId = ts.Id
                                where t.AssociatedToId = @Id";

        CommandDefinition commandDefinition = new(commandText: query, parameters: new { id }, cancellationToken: cancellationToken);

        return await connection.QueryAsync<GoalTaskDto>(commandDefinition);
    }

    private static async Task CheckIfUserHasAccessToGoal(Guid userId, Guid id, SqlConnection connection, CancellationToken cancellationToken)
    {
        const string authorizedQuery = @"select g.Id, g.CreatedBy 
                                         from Goals g 
                                         where g.Id = @Id";

        CommandDefinition authorizationCommandDefinition = new(commandText: authorizedQuery, parameters: new { id }, cancellationToken: cancellationToken);

        dynamic result = await connection.QueryFirstOrDefaultAsync<dynamic>(authorizationCommandDefinition) ??
                 throw new KeyNotFoundException();

        Guid createdBy = (Guid)result.CreatedBy;
        if (createdBy != userId)
            throw new UnauthorizedAccessException();
    }

    public async Task<IEnumerable<GoalDto>> GetGoals(Guid userId,
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
