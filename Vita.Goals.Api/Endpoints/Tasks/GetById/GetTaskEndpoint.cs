using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MinimalApi.Endpoint;
using Swashbuckle.AspNetCore.Annotations;
using Vita.Goals.Application.Queries.Tasks;

namespace Vita.Goals.Api.Endpoints.Tasks.GetById;
internal class GetTaskEndpoint : IEndpoint<IResult, Guid, CancellationToken>
{
    private readonly ITaskQueryStore _taskQueryStore;

    public GetTaskEndpoint(ITaskQueryStore taskQueryStore)
    {
        _taskQueryStore = taskQueryStore;
    }

    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/tasks/{id:guid}", (Guid id, CancellationToken cancellationToken) => HandleAsync(id, cancellationToken))
           .Produces<TaskDto>()
           .ProducesProblem(StatusCodes.Status404NotFound)
           .ProducesProblem(StatusCodes.Status401Unauthorized)
           .WithMetadata(new SwaggerOperationAttribute())
           .WithTags("Tasks")
           .RequireAuthorization();
    }

    public async Task<IResult> HandleAsync(Guid id, CancellationToken cancellationToken)
    {
        TaskDto task = await _taskQueryStore.GetTaskById(id, cancellationToken);

        if (task is null)
            return Results.NotFound();

        return Results.Ok(task);
    }
}
