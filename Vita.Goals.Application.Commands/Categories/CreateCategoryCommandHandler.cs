using MediatR;
using Vita.Goals.Domain.Aggregates.Categories;
using Vita.Goals.Domain.ValueObjects;

namespace Vita.Goals.Application.Commands.Categories
{
    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Guid>
    {
        private readonly ICategoriesRepository _categoriesRepository;

        public CreateCategoryCommandHandler(ICategoriesRepository categoriesRepository)
        {
            _categoriesRepository = categoriesRepository;
        }

        public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = new Category(request.Name, new HexColor(request.Color), request.CreatedBy);

            await _categoriesRepository.Add(category);
            await _categoriesRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            return category.Id;
        }
    }
}
