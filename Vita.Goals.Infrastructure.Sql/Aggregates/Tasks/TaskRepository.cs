using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vita.Core.Domain.Repositories;
using Vita.Goals.Domain.Aggregates.Tasks;

namespace Vita.Goals.Infrastructure.Sql.Aggregates.Tasks;

public class TaskRepository : ITaskRepository
{
    private readonly GoalsDbContext _context;
    public IUnitOfWork UnitOfWork => _context;

    public TaskRepository(GoalsDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Domain.Aggregates.Tasks.Task> FindById(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Tasks.Include(x => x.AssociatedTo).FirstOrDefaultAsync(x => x.Id == id, cancellationToken) ??
               throw new KeyNotFoundException();
    }

    public Task<Domain.Aggregates.Tasks.Task> Add(Domain.Aggregates.Tasks.Task task)
    {
        var entry = _context.Tasks.Add(task);
        return System.Threading.Tasks.Task.FromResult(entry.Entity);
    }

    public Task<Domain.Aggregates.Tasks.Task> Update(Domain.Aggregates.Tasks.Task task)
    {
        var entry = _context.Tasks.Update(task);
        return System.Threading.Tasks.Task.FromResult(entry.Entity);
    }

    public async System.Threading.Tasks.Task Delete(Guid id)
    {
        var task = await FindById(id);
        _context.Remove(task);
    }

    public IEnumerable<Domain.Aggregates.Tasks.Task> Get(Func<Domain.Aggregates.Tasks.Task, bool> taskSpecificationFilter)
    {
        return _context.Tasks.Where(taskSpecificationFilter);
    }
}
