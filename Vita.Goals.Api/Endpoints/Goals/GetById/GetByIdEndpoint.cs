using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MinimalApi.Endpoint;
using Swashbuckle.AspNetCore.Annotations;
using Vita.Goals.Application.Queries.Goals;

namespace Vita.Goals.Api.Endpoints.Goals.GetById;
internal class GetByIdEndpoint : IEndpoint<IResult, Guid, CancellationToken>
{
    private readonly IGoalQueryStore _goalQueryStore;

    public GetByIdEndpoint(IGoalQueryStore goalQueryStore)
    {
        _goalQueryStore = goalQueryStore;
    }

    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/goals/{id:guid}", (Guid id, CancellationToken cancellationToken) => HandleAsync(id, cancellationToken))
           .Produces<GoalDto>()
           .ProducesProblem(StatusCodes.Status404NotFound)
           .ProducesProblem(StatusCodes.Status401Unauthorized)
           .WithMetadata(new SwaggerOperationAttribute())
           .WithTags("Goals")
           .RequireAuthorization();
    }

    public async Task<IResult> HandleAsync(Guid id, CancellationToken cancellationToken)
    {
        GoalDto goal = await _goalQueryStore.GetGoalById(id, cancellationToken);

        if (goal == null)
            return Results.NotFound();

        return Results.Ok(goal);
    }
}
