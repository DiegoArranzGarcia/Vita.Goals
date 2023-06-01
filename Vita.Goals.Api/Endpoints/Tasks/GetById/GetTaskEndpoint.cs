using FastEndpoints;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Vita.Goals.Application.Queries.Tasks;

namespace Vita.Goals.Api.Endpoints.Tasks.GetById;
public class GetTaskEndpoint : Endpoint<EmptyRequest, TaskDto>
{
    private readonly ITaskQueryStore _taskQueryStore;

    public GetTaskEndpoint(ITaskQueryStore taskQueryStore)
    {
        _taskQueryStore = taskQueryStore;
    }

    /// <inheritdoc/>
    public override void Configure()
    {
        Get("tasks/{id}");
        Tags("Tasks");
        Policies("ApiScope");
        Description(x => x.Produces<TaskDto>()
                          .ProducesProblem(StatusCodes.Status404NotFound)
                          .ProducesProblem(StatusCodes.Status401Unauthorized)
                          .WithTags("Tasks"));
    }

    public async override Task HandleAsync(EmptyRequest request, CancellationToken ct)
    {
        if (!Guid.TryParse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out Guid userId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        TaskDto task = await _taskQueryStore.GetTaskById(userId, Route<Guid>("id"), ct);

        if (task == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SendOkAsync(task, ct);
    }
}
