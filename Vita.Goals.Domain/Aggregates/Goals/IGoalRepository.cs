using System;
using System.Threading;
using System.Threading.Tasks;
using Vita.Core.Domain.Repositories;

namespace Vita.Goals.Domain.Aggregates.Goals;

public interface IGoalsRepository : IRepository<Goal>
{
    Task<Goal> Add(Goal goal);
    Task<Goal> Update(Goal goal);
    Task Delete(Guid id);
    Task<Goal> FindById(Guid id, CancellationToken cancellationToken = default);
}
