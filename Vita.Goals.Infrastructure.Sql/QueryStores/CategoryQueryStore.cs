using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vita.Goals.Application.Queries.Categories;
using Vita.Goals.Infrastructure.Sql.QueryStores.Configuration;

namespace Vita.Goals.Infrastructure.Sql.QueryStores
{
    public class CategoryQueryStore : ICategoryQueryStore
    {
        private const string GetCategoriesCreatedByUserQuery = "select Id, Name, Color from Categories where CreatedBy = @UserId";
        private const string GetCategoryByIdQuery = "select Id, Name, Color from Categories where Id = @Id";

        private readonly IConnectionStringProvider _connectionStringProvider;

        public CategoryQueryStore(IConnectionStringProvider connectionStringProvider)
        {
            _connectionStringProvider = connectionStringProvider ?? throw new ArgumentNullException(nameof(connectionStringProvider));
        }

        public async Task<IEnumerable<CategoryDto>> GetCategoriesCreatedByUser(Guid userId)
        {
            using var connection = new SqlConnection(_connectionStringProvider.ConnectionString);
            connection.Open();

            return await connection.QueryAsync<CategoryDto>(GetCategoriesCreatedByUserQuery, new { UserId = userId });
        }

        public async Task<CategoryDto> GetCategoryById(Guid id)
        {

            using var connection = new SqlConnection(_connectionStringProvider.ConnectionString);
            connection.Open();

            return await connection.QueryFirstOrDefaultAsync<CategoryDto>(GetCategoryByIdQuery, new { id });
        }
    }
}
