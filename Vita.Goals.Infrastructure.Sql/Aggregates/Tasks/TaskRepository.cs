using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vita.Core.Domain.Repositories;

using DomainTask = Vita.Goals.Domain.Aggregates.Tasks.Task;

namespace Vita.Goals.Infrastructure.Sql.Aggregates.Tasks;

public class TaskRepository : Domain.Aggregates.Tasks.ITaskRepository
{
    private readonly GoalsDbContext _context;
    public IUnitOfWork UnitOfWork => _context;

    public TaskRepository(GoalsDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<DomainTask> FindById(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Tasks.Include(x => x.AssociatedTo).FirstOrDefaultAsync(x => x.Id == id, cancellationToken) ??
               throw new KeyNotFoundException();
    }

    public Task Add(DomainTask task)
    {
        var entry = _context.Tasks.Add(task);
        return Task.FromResult(entry.Entity);
    }

    public Task Update(DomainTask task)
    {
        var entry = _context.Tasks.Update(task);
        return Task.FromResult(entry.Entity);
    }

    public async Task Delete(Guid id)
    {
        var task = await FindById(id);
        _context.Remove(task);
    }

    public IEnumerable<DomainTask> Get(Func<DomainTask, bool> taskSpecificationFilter)
    {
        return _context.Tasks.Where(taskSpecificationFilter);
    }
}
