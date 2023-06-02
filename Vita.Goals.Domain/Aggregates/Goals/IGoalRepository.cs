using System;
using System.Threading;
using System.Threading.Tasks;
using Vita.Core.Domain.Repositories;

namespace Vita.Goals.Domain.Aggregates.Goals;

public interface IGoalRepository : IRepository<Goal>
{
    Task Add(Goal goal);
    Task Update(Goal goal);
    Task Delete(Guid id);
    Task<Goal> FindById(Guid id, CancellationToken cancellationToken = default);
}
