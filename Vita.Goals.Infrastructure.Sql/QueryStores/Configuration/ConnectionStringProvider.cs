using System;

namespace Vita.Goals.Infrastructure.Sql.QueryStores.Configuration
{
    public class ConnectionStringProvider : IConnectionStringProvider
    {
        private readonly string _connectionString;

        public ConnectionStringProvider(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public string ConnectionString => _connectionString;
    }
}
