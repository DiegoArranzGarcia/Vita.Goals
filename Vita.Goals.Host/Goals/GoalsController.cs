using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Vita.Goals.Application.Commands.Goals;
using Vita.Goals.Application.Queries.Goals;

namespace Vita.Goals.Host.Goals
{
    [Authorize]
    [ApiController] 
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
        public async Task<IActionResult> GetGoals(bool? showCompleted = null, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null)
        {
            if (!Guid.TryParse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out Guid userId))
                return Unauthorized();

            var goals = await _goalQueryStore.GetGoalsCreatedByUser(userId, showCompleted, startDate, endDate);

            return Ok(goals);
        }

        [HttpGet]
        [Route("{id}", Name = nameof(GetGoal))]
        public async Task<IActionResult> GetGoal(Guid id)
        {
            var goal = await _goalQueryStore.GetGoalById(id);

            if (goal == null)
                return NotFound();

            return Ok(goal);
        }

        [HttpGet]
        [Route("{id}/tasks")]
        public async Task<IActionResult> GetGoalTasks(Guid id)
        {
            IEnumerable<GoalTaskDto> tasks = await _goalQueryStore.GetGoalTasks(id);

            return Ok(tasks);
        }

        [HttpPost]
        public async Task<IActionResult> CreateGoal(CreateGoalCommand createGoalCommand)
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

            return CreatedAtRoute(routeName: nameof(GetGoal), routeValues: new { id = createdGoal }, value: null);
        }

        [HttpPatch]
        [Route("{id}")]
        public async Task<IActionResult> UpdateGoalAsync(Guid id, UpdateGoalCommand updateGoalCommand)
        {
            if (!Guid.TryParse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out Guid userId))
                return Unauthorized();

            UpdateGoalCommand command = updateGoalCommand with { Id = id };

            await _mediator.Send(command);

            return NoContent();
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteGoal(Guid id)
        {
            if (!Guid.TryParse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out Guid userId))
                return Unauthorized();

            DeleteGoalCommand deleteGoalCommand = new() { Id = id };
            await _mediator.Send(deleteGoalCommand);

            return NoContent();
        }

        [HttpPut]
        [Route("{id}/complete")]
        public async Task<IActionResult> CompleteGoal(Guid id)
        {
            if (!Guid.TryParse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out Guid userId))
                return Unauthorized();

            CompleteGoalCommand completeGoalCommand = new() { Id = id };
            await _mediator.Send(completeGoalCommand);

            return NoContent();
        }

        [HttpPut]
        [Route("{id}/ready")]
        public async Task<IActionResult> ReadyGoal(Guid id)
        {
            if (!Guid.TryParse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out Guid userId))
                return Unauthorized();

            InProgressGoalCommand readyGoalCommand = new() { Id = id };
            await _mediator.Send(readyGoalCommand);

            return NoContent();
        }

        [HttpPut]
        [Route("{id}/in-progress")]
        public async Task<IActionResult> InProgressGoal(Guid id)
        {
            if (!Guid.TryParse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out Guid userId))
                return Unauthorized();

            InProgressGoalCommand inProgressGoalCommand = new() { Id = id };
            await _mediator.Send(inProgressGoalCommand);

            return NoContent();
        }
    }
}