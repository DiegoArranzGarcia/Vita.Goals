namespace Vita.Goals.Infrastructure.Sql.QueryStores.Configuration;

public interface IConnectionStringProvider
{
    string ConnectionString { get; }
}
