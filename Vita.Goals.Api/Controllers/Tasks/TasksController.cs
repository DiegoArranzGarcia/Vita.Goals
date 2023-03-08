using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vita.Goals.Application.Commands.Tasks;
using MediatR;
using System.Security.Claims;
using Vita.Goals.Application.Queries.Tasks;

namespace Vita.Goals.Api.Controllers.Tasks;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ITaskQueryStore _taskQueryStore;

    public TasksController(IMediator mediator, ITaskQueryStore taskQueryStore)
    {
        _sender = mediator;
        _taskQueryStore = taskQueryStore;
    }

    [HttpGet]
    [Route("{id}", Name = nameof(GetTask))]
    public async Task<IActionResult> GetTask(Guid id)
    {
        TaskDto task = await _taskQueryStore.GetTaskById(id);

        if (task is null)
            return NotFound();

        return Ok(task);
    }

    [HttpGet]
    public async Task<IActionResult> GetTasks(Guid? userId = null, string? status = null, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null)
    {
        bool hasClaimUserIdClaim = Guid.TryParse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out Guid claimUserId);

        if (userId.HasValue && hasClaimUserIdClaim && userId != claimUserId)
            return Unauthorized();

        IEnumerable<TaskDto> tasks = await _taskQueryStore.GetTasksCreatedByUser(userId ?? claimUserId, status, startDate, endDate);

        return Ok(tasks);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTask(CreateTaskDto dto)
    {
        if (!Guid.TryParse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out Guid userId))
            return Unauthorized();

        CreateTaskCommand command = new(dto.GoalId, dto.Title, dto.PlannedDateStart, dto.PlannedDateEnd);        
        Guid taskId = await _sender.Send(command);

        Response.Headers.Add("Access-Control-Allow-Headers", "Location");
        Response.Headers.Add("Access-Control-Expose-Headers", "Location");

        return CreatedAtRoute(routeName: nameof(GetTask), routeValues: new { id = taskId }, value: null);
    }

    [HttpPatch]
    [Route("{id}")]
    public async Task<IActionResult> UpdateTask(Guid id, UpdateTaskDto updateTaskDto)
    {
        if (!Guid.TryParse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out Guid userId))
            return Unauthorized();

        UpdateTaskCommand command = new(id, updateTaskDto.Title, updateTaskDto.GoalId, updateTaskDto.PlannedDateEnd, updateTaskDto.PlannedDateStart);
        await _sender.Send(command);

        return NoContent();
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<IActionResult> DeleteTask(Guid id)
    {
        if (!Guid.TryParse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out Guid userId))
            return Unauthorized();

        DeleteTaskCommand command = new(id);
        await _sender.Send(command);

        return NoContent();
    }
}
