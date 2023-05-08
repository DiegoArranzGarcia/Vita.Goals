using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vita.Core.Domain.Repositories;

namespace Vita.Goals.Domain.Aggregates.Tasks;

public interface ITaskRepository : IRepository<Task>
{
    Task<Task> Add(Task goal);
    Task<Task> Update(Task goal);
    System.Threading.Tasks.Task Delete(Guid id);
    Task<Task> FindById(Guid id, System.Threading.CancellationToken cancellationToken);
    IEnumerable<Task> Get(Func<Task, bool> taskSpecificationFilter);
}
