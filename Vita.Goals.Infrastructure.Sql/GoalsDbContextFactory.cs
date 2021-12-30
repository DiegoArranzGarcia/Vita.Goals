using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Vita.Goals.Infrastructure.Sql
{
    public class GoalsDbContextFactory : IDesignTimeDbContextFactory<GoalsDbContext>
    {
        public GoalsDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<GoalsDbContext>();
            optionsBuilder.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Vita.Goals.Development;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");

            return new GoalsDbContext(optionsBuilder.Options, null);
        }
    }
}
