using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Vita.Goals.Application.Commands.Goals;
using Vita.Goals.Application.Queries.Goals;

namespace Vita.Goals.Host.Goals
{
    [ApiController]
    [Authorize]
    [Route("api/goals")]
    public class GoalsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IGoalQueryStore _goalQueryStore;

        public GoalsController(IMediator mediator, IGoalQueryStore goalQueryStore)
        {
            _mediator = mediator;
            _goalQueryStore = goalQueryStore;
        }

        [HttpGet]
        public async Task<IActionResult> GetGoalsAsync(bool? showCompleted = null, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null)
        {
            if (!Guid.TryParse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out Guid userId))
                return Unauthorized();

            var goals = await _goalQueryStore.GetGoalsCreatedByUser(userId, showCompleted, startDate, endDate);

            return Ok(goals);
        }

        [HttpGet]
        [Route("{id}", Name = nameof(GetGoalAsync))]
        public async Task<IActionResult> GetGoalAsync(Guid id)
        {
            var goal = await _goalQueryStore.GetGoalById(id);

            if (goal == null)
                return NotFound();

            return Ok(goal);
        }

        [HttpPost]
        public async Task<IActionResult> CreateGoalAsync(CreateGoalCommand createGoalCommand)
        {
            if (!Guid.TryParse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out Guid userId))
                return Unauthorized();

            if (userId != createGoalCommand.CreatedBy)
                return Forbid();

            if (string.IsNullOrEmpty(createGoalCommand.Title))
                return BadRequest("The title cannot be empty");

            var createdGoal = await _mediator.Send(createGoalCommand);

            Response.Headers.Add("Access-Control-Allow-Headers", "Location");
            Response.Headers.Add("Access-Control-Expose-Headers", "Location");

            return CreatedAtRoute(routeName: nameof(GetGoalAsync), routeValues: new { id = createdGoal }, value: null);
        }


        [HttpPost]
        [Route("{id}/complete")]
        public async Task<IActionResult> CompleteGoalAsync(Guid id)
        {
            if (!Guid.TryParse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out Guid userId))
                return Unauthorized();

            var completeGoalCommand = new CompleteGoalCommand() { Id = id };
            await _mediator.Send(completeGoalCommand);

            return NoContent();
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateGoalAsync(Guid id, UpdateGoalCommand updateGoalCommand)
        {
            if (!Guid.TryParse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out Guid userId))
                return Unauthorized();

            updateGoalCommand.Id = id;
            await _mediator.Send(updateGoalCommand);

            return NoContent();
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteGoalAsync(Guid id)
        {
            if (!Guid.TryParse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out Guid userId))
                return Unauthorized();

            var deleteGoalCommand = new DeleteGoalCommand() { Id = id };
            await _mediator.Send(deleteGoalCommand);

            return NoContent();
        }
    }
}