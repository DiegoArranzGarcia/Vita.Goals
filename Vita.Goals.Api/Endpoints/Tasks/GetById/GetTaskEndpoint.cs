using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Vita.Goals.Application.Queries.Tasks;

namespace Vita.Tasks.Api.Endpoints.Tasks.GetById;
public class GetTaskEndpoint : Endpoint<Guid, TaskDto>
{
    private readonly ITaskQueryStore _taskQueryStore;

    public GetTaskEndpoint(ITaskQueryStore taskQueryStore)
    {
        _taskQueryStore = taskQueryStore;
    }

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

    public async override Task HandleAsync(Guid id, CancellationToken cancellationToken)
    {
        TaskDto task = await _taskQueryStore.GetTaskById(id, cancellationToken);

        if (task == null)
        {
            await SendNotFoundAsync(cancellationToken);
            return;
        }

        await SendOkAsync(task, cancellationToken);
    }
}
