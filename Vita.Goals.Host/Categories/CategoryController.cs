using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Vita.Goals.Application.Commands.Categories;
using Vita.Goals.Application.Queries.Categories;

namespace Vita.Goals.Host.Categories
{
    [ApiController]
    [Authorize]
    [Route("api/categories")]
    public class CategoryController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICategoryQueryStore _categoryQueryStore;

        public CategoryController(IMediator mediator, ICategoryQueryStore categoryQueryStore)
        {
            _mediator = mediator;
            _categoryQueryStore = categoryQueryStore;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategoriesAsync()
        {
            if (!Guid.TryParse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out Guid userId))
                return Unauthorized();

            var categories = await _categoryQueryStore.GetCategoriesCreatedByUser(userId);

            return Ok(categories);
        }

        [HttpGet]
        [Route("{id}", Name = nameof(GetCategoryAsync))]
        public async Task<IActionResult> GetCategoryAsync(Guid id)
        {
            var category = await _categoryQueryStore.GetCategoryById(id);

            if (category == null)
                return NotFound();

            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(CreateCategoryCommand createCategoryCommand)
        {
            if (!Guid.TryParse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out Guid userId))
                return Unauthorized();

            if (userId != createCategoryCommand.CreatedBy)
                return Forbid();

            var createdCategory = await _mediator.Send(createCategoryCommand);
            return CreatedAtRoute(routeName: nameof(GetCategoryAsync), routeValues: new { id = createdCategory }, value: null);
        }
    }
}