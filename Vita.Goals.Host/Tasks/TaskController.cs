using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using System;
using Vita.Goals.Application.Commands.Tasks;
using MediatR;
using System.Security.Claims;
using Vita.Goals.Application.Commands.Goals;
using Vita.Goals.Application.Queries.Goals;
using Vita.Goals.Application.Queries.Tasks;
using Vita.Goals.Infrastructure.Sql.QueryStores;
using System.Collections;
using System.Collections.Generic;

namespace Vita.Goals.Host.Tasks
{
    [ApiController]
    [Authorize]
    [Route("api/tasks")]
    public class TaskController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ITaskQueryStore _taskQueryStore;

        public TaskController(IMediator mediator, ITaskQueryStore taskQueryStore)
        {
            _mediator = mediator;
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
        public async Task<IActionResult> GetTasks(bool? showCompleted = null, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null)
        {
            if (!Guid.TryParse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out Guid userId))
                return Unauthorized();

            IEnumerable<TaskDto> tasks = await _taskQueryStore.GetTasksCreatedByUser(userId, showCompleted, startDate, endDate);

            return Ok(tasks);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask(CreateTaskCommand command)
        {
            if (!Guid.TryParse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out Guid userId))
                return Unauthorized();

            Guid taskId = await _mediator.Send(command);

            Response.Headers.Add("Access-Control-Allow-Headers", "Location");
            Response.Headers.Add("Access-Control-Expose-Headers", "Location");

            return CreatedAtRoute(routeName: nameof(GetTask), routeValues: new { id = taskId }, value: null);
        }

        [HttpPatch]
        [Route("{id}")]
        public async Task<IActionResult> UpdateTask(Guid id, UpdateTaskCommand updateTaskCommand)
        {
            if (!Guid.TryParse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out Guid userId))
                return Unauthorized();

            UpdateTaskCommand command = updateTaskCommand with { TaskId = id };

            await _mediator.Send(command);

            return NoContent();
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            if (!Guid.TryParse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out Guid userId))
                return Unauthorized();

            DeleteTaskCommand command = new() { Id = id };
            await _mediator.Send(command);

            return NoContent();
        }
    }
}
