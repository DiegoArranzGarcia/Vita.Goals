using System;
using System.Threading.Tasks;
using Vita.Core.Domain.Repositories;

namespace Vita.Goals.Domain.Aggregates.Categories
{
    public interface ICategoriesRepository : IRepository<Category>
    {
        Task<Category> Add(Category category);
        Task<Category> Update(Category category);
        Task<Category> FindByIdAsync(Guid id);
    }
}
