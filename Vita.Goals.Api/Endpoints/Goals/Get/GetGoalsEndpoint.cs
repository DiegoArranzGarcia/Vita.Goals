using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MinimalApi.Endpoint;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using Vita.Goals.Application.Queries.Goals;

namespace Vita.Goals.Api.Endpoints.Goals.GetGoals;
internal class GetGoalsEndpoint : IEndpoint<IResult, GetGoalsRequest, ClaimsPrincipal, CancellationToken>
{
    private readonly IGoalQueryStore _goalQueryStore;

    public GetGoalsEndpoint(IGoalQueryStore goalQueryStore)
    {
        _goalQueryStore = goalQueryStore;
    }

    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/goals", ([AsParameters] GetGoalsRequest request, ClaimsPrincipal user, CancellationToken cancellationToken) => HandleAsync(request, user, cancellationToken))
           .Produces<IEnumerable<GoalDto>>()
           .ProducesProblem(StatusCodes.Status401Unauthorized)
           .WithMetadata(new SwaggerOperationAttribute())
           .WithTags("Goals")
           .RequireAuthorization();
    }

    public async Task<IResult> HandleAsync(GetGoalsRequest request, ClaimsPrincipal user, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out Guid userId))
            return Results.Unauthorized();

        IEnumerable<GoalDto> goals = await _goalQueryStore.GetGoalsCreatedByUser(userId, request.ShowCompleted, request.StartDate, request.EndDate, cancellationToken);

        return Results.Ok(goals);
    }
}
