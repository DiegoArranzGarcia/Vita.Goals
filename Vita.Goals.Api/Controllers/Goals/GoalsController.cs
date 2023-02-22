using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Vita.Goals.Application.Commands.Goals;
using Vita.Goals.Application.Queries.Goals;

namespace Vita.Goals.Api.Controllers.Goals;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class GoalsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IGoalQueryStore _goalQueryStore;

    /// <summary>
    /// Goals API operations
    /// </summary>
    /// <param name="mediator"></param>
    /// <param name="goalQueryStore"></param>
    public GoalsController(IMediator mediator, IGoalQueryStore goalQueryStore)
    {
        _sender = mediator;
        _goalQueryStore = goalQueryStore;
    }
    
    /// <summary>
    /// Gets goals that meets the filtering options
    /// </summary>
    /// <param name="showCompleted"></param>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetGoals(bool? showCompleted = null, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out Guid userId))
            return Unauthorized();

        var goals = await _goalQueryStore.GetGoalsCreatedByUser(userId, showCompleted, startDate, endDate, cancellationToken);

        return Ok(goals);
    }

    [HttpGet]
    [Route("{id}", Name = nameof(GetGoal))]
    public async Task<IActionResult> GetGoal(Guid id, CancellationToken cancellationToken = default)
    {
        var goal = await _goalQueryStore.GetGoalById(id, cancellationToken);

        if (goal == null)
            return NotFound();

        return Ok(goal);
    }

    [HttpGet]
    [Route("{id}/tasks")]
    public async Task<IActionResult> GetGoalTasks(Guid id, CancellationToken cancellationToken = default)
    {
        IEnumerable<GoalTaskDto> tasks = await _goalQueryStore.GetGoalTasks(id, cancellationToken);

        return Ok(tasks);
    }

    [HttpPost]
    public async Task<IActionResult> CreateGoal(CreateGoalDto createGoalDto, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out Guid userId))
            return Unauthorized();

        CreateGoalCommand command = new(createGoalDto.Title, createGoalDto.Description, userId, createGoalDto.AimDateStart, createGoalDto.AimDateEnd);

        Guid createdGoalId = await _sender.Send(command, cancellationToken);

        Response.Headers.Add("Access-Control-Allow-Headers", "Location");
        Response.Headers.Add("Access-Control-Expose-Headers", "Location");

        return CreatedAtRoute(routeName: nameof(GetGoal), routeValues: new { id = createdGoalId }, value: null);
    }

    [HttpPatch]
    [Route("{id}")]
    public async Task<IActionResult> UpdateGoalAsync(Guid id, UpdateGoalDto updateGoalDto, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out Guid userId))
            return Unauthorized();

        UpdateGoalCommand command = new(id, updateGoalDto.Title, updateGoalDto.Description, updateGoalDto.AimDateStart, updateGoalDto.AimDateEnd);
        await _sender.Send(command, cancellationToken);

        return NoContent();
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<IActionResult> DeleteGoal(Guid id, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out Guid userId))
            return Unauthorized();

        DeleteGoalCommand deleteGoalCommand = new(id);
        await _sender.Send(deleteGoalCommand, cancellationToken);

        return NoContent();
    }

    [HttpPut]
    [Route("{id}/complete")]
    public async Task<IActionResult> CompleteGoal(Guid id, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out Guid userId))
            return Unauthorized();

        CompleteGoalCommand completeGoalCommand = new(id);
        await _sender.Send(completeGoalCommand, cancellationToken);

        return NoContent();
    }

    [HttpPut]
    [Route("{id}/ready")]
    public async Task<IActionResult> ReadyGoal(Guid id, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out Guid userId))
            return Unauthorized();

        InProgressGoalCommand readyGoalCommand = new(id);
        await _sender.Send(readyGoalCommand, cancellationToken);

        return NoContent();
    }

    [HttpPut]
    [Route("{id}/in-progress")]
    public async Task<IActionResult> InProgressGoal(Guid id, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out Guid userId))
            return Unauthorized();

        InProgressGoalCommand inProgressGoalCommand = new(id);
        await _sender.Send(inProgressGoalCommand, cancellationToken);

        return NoContent();
    }
}