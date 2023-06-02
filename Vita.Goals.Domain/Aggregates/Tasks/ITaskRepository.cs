using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vita.Core.Domain.Repositories;

namespace Vita.Goals.Domain.Aggregates.Tasks;

public interface ITaskRepository : IRepository<Task>
{
    System.Threading.Tasks.Task Add(Task task);
    System.Threading.Tasks.Task Update(Task task);
    System.Threading.Tasks.Task Delete(Guid id);
    Task<Task> FindById(Guid id, System.Threading.CancellationToken cancellationToken = default);
    IEnumerable<Task> Get(Func<Task, bool> taskSpecificationFilter);
}
