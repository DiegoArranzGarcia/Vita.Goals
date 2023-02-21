using System;
using System.Threading;
using System.Threading.Tasks;
using Vita.Core.Domain.Repositories;
using Vita.Goals.Domain.Aggregates.Goals;
using Task = System.Threading.Tasks.Task;

namespace Vita.Goals.Infrastructure.Sql.Aggregates.Goals;

public class GoalsRepository : IGoalsRepository
{
    private readonly GoalsDbContext _context;
    public IUnitOfWork UnitOfWork => _context;

    public GoalsRepository(GoalsDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Goal> FindById(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Goals.FindAsync(id, cancellationToken);
    }

    public Task<Goal> Add(Goal goal)
    {
        var entry = _context.Goals.Add(goal);
        return Task.FromResult(entry.Entity);
    }

    public Task<Goal> Update(Goal goal)
    {
        var entry = _context.Goals.Update(goal);
        return Task.FromResult(entry.Entity);
    }

    public async Task Delete(Guid id)
    {
        var goal = await FindById(id);
        _context.Remove(goal);
    }
}
