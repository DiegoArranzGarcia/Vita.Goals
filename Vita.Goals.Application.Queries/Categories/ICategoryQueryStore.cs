using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vita.Goals.Application.Queries.Categories
{
    public interface ICategoryQueryStore
    {
        Task<IEnumerable<CategoryDto>> GetCategoriesCreatedByUser(Guid userId);
        Task<CategoryDto> GetCategoryById(Guid id);
    }
}
