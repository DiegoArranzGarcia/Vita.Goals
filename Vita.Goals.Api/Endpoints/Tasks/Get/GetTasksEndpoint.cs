using FastEndpoints;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Vita.Goals.Application.Queries.Tasks;

namespace Vita.Goals.Api.Endpoints.Tasks.Get;
internal class GetTasksEndpoint : Endpoint<GetTasksRequest, IEnumerable<TaskDto>>
{
    private readonly ITaskQueryStore _taskQueryStore;

    public GetTasksEndpoint(ITaskQueryStore taskQueryStore)
    {
        _taskQueryStore = taskQueryStore;
    }

    public override void Configure()
    {
        Get("tasks");
        Policies("ApiScope");
        Description(x => x.Produces<IEnumerable<TaskDto>>()
                          .ProducesProblem(StatusCodes.Status401Unauthorized)
                          .WithTags("Tasks"));
    }

    public async override Task HandleAsync(GetTasksRequest request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out Guid userId))
        {
            await SendUnauthorizedAsync(cancellationToken);
            return;
        }

        IEnumerable<TaskDto> tasks = await _taskQueryStore.GetTasksCreatedByUser(userId, request.Status, request.StartDate, request.EndDate, cancellationToken);

        await SendOkAsync(tasks, cancellationToken);
    }
}
