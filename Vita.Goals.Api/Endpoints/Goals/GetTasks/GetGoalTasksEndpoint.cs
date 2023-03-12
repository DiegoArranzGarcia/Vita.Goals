using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MinimalApi.Endpoint;
using Swashbuckle.AspNetCore.Annotations;
using Vita.Goals.Application.Queries.Goals;

namespace Vita.Goals.Api.Endpoints.Goals.GetGoalTasks;
internal class GetGoalTasksEndpoint : IEndpoint<IResult, Guid, CancellationToken>
{
    private readonly IGoalQueryStore _goalQueryStore;

    public GetGoalTasksEndpoint(IGoalQueryStore goalQueryStore)
    {
        _goalQueryStore = goalQueryStore;
    }

    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/goals/{id:guid}/tasks", (Guid id, CancellationToken cancellationToken) => HandleAsync(id, cancellationToken))
           .Produces<IEndpoint<GoalTaskDto>>()
           .ProducesProblem(StatusCodes.Status404NotFound)
           .ProducesProblem(StatusCodes.Status401Unauthorized)
           .WithMetadata(new SwaggerOperationAttribute())
           .WithTags("Goals")
           .RequireAuthorization();
    }

    public async Task<IResult> HandleAsync(Guid id, CancellationToken cancellationToken)
    {
        IEnumerable<GoalTaskDto> tasks = await _goalQueryStore.GetGoalTasks(id, cancellationToken);

        return Results.Ok(tasks);
    }
}
