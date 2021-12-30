﻿using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vita.Goals.Application.Queries.Goals;
using Vita.Goals.Domain.Aggregates.Goals;
using Vita.Goals.Infrastructure.Sql.QueryStores.Configuration;

namespace Vita.Goals.Infrastructure.Sql.QueryStores
{
    public class GoalQueryStore : IGoalQueryStore
    {
        private const string GetGoalByIdQuery = @"select g.Id, g.Title, g.Description, g.CreatedOn, g.AimDate_Start as AimDateStart, g.AimDate_End as AimDateEnd, gs.Name as Status
                                                    from Goals g 
                                              inner join GoalStatus gs on g.GoalStatusId = gs.Id
                                                   where g.Id = @Id";

        private const string GetGoalsCreatedByUserQuery = @"select g.Id, g.Title, g.Description, g.CreatedOn, g.AimDate_Start as AimDateStart, g.AimDate_End as AimDateEnd, gs.Name as Status
                                                              from Goals g
                                                        inner join GoalStatus gs on g.GoalStatusId = gs.Id
                                                             where CreatedBy = @UserId";

        private readonly IConnectionStringProvider _connectionStringProvider;

        public GoalQueryStore(IConnectionStringProvider connectionStringProvider)
        {
            _connectionStringProvider = connectionStringProvider ?? throw new ArgumentNullException(nameof(connectionStringProvider));
        }

        public async Task<GoalDto> GetGoalById(Guid id)
        {
            using var connection = new SqlConnection(_connectionStringProvider.ConnectionString);
            connection.Open();

            return await connection.QueryFirstOrDefaultAsync<GoalDto>(GetGoalByIdQuery, new { id });
        }

        public async Task<IEnumerable<GoalDto>> GetGoalsCreatedByUser(Guid userId,
                                                                      bool? showCompleted,
                                                                      DateTimeOffset? startDate,
                                                                      DateTimeOffset? endDate)
        {
            using var connection = new SqlConnection(_connectionStringProvider.ConnectionString);
            connection.Open();

            var sqlQuery = GetGoalsCreatedByUserQuery;

            if (startDate.HasValue && endDate.HasValue)
                sqlQuery += $@" and (@Start <= g.AimDate_End and g.AimDate_Start <= @End)";

            if (!showCompleted.HasValue || !showCompleted.Value)
                sqlQuery += $@" and (g.GoalStatusId = {GoalStatus.ToDo.Id})";

            return await connection.QueryAsync<GoalDto>(sqlQuery, new { UserId = userId, Start = startDate, End = endDate });
        }
    }
}
