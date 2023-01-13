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

namespace Vita.Goals.Host.Tasks
{
    [ApiController]
    [Authorize]
    [Route("api/tasks")]
    public class TaskController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TaskController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("{id}", Name = nameof(GetTask))]
        public IActionResult GetTask(Guid id)
        {
            var taskDto = new TaskDto() { TaskId = id };

            return Ok(taskDto);
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

            UpdateTaskCommand command = updateTaskCommand with { Id = id };

            await _mediator.Send(command);

            return NoContent();
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            if (!Guid.TryParse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out Guid userId))
                return Unauthorized();

            DeleteGoalCommand command = new() { Id = id };
            await _mediator.Send(command);

            return NoContent();
        }
    }
}
