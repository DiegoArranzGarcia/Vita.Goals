using System;
using System.Collections.Generic;
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
        return await _context.Goals.FindAsync(new object[] { id }, cancellationToken) ??
               throw new KeyNotFoundException();
    }

    public Task Add(Goal goal)
    {
        _context.Goals.Add(goal);

        return Task.CompletedTask;
    }

    public Task Update(Goal goal)
    {
        _context.Goals.Update(goal);

        return Task.CompletedTask;
    }

    public async Task Delete(Guid id)
    {
        var goal = await FindById(id);
        _context.Remove(goal);
    }
}
