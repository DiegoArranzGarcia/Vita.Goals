using MediatR;
using Microsoft.EntityFrameworkCore;
using Vita.Core.Infrastructure.Sql;
using Vita.Goals.Domain.Aggregates.Goals;
using Vita.Goals.Infrastructure.Sql.Aggregates.Goals;
using Vita.Goals.Infrastructure.Sql.Aggregates.Tasks;

namespace Vita.Goals.Infrastructure.Sql;

public class GoalsDbContext : VitaDbContext
{
    private readonly IMediator _mediator;

    public DbSet<Goal> Goals { get; private set; }
    public DbSet<GoalStatus> GoalStatuses { get; private set; }
    public DbSet<Domain.Aggregates.Tasks.Task> Tasks { get; private set; }
    public DbSet<Domain.Aggregates.Tasks.TaskStatus> TaskStatuses { get; private set; }

    public GoalsDbContext(DbContextOptions options, IMediator mediator) : base(options, mediator)
    {
        _mediator = mediator;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {       
        modelBuilder.ApplyConfiguration(new GoalEntityConfiguration());
        modelBuilder.ApplyConfiguration(new GoalStatusSafeEnumConfiguration());
        modelBuilder.ApplyConfiguration(new TaskEntityConfiguration());
        modelBuilder.ApplyConfiguration(new TaskStatusSafeEnumConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}
