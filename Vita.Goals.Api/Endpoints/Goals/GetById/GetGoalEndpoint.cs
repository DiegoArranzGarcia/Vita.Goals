using FastEndpoints;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Vita.Goals.Application.Queries.Goals;

namespace Vita.Goals.Api.Endpoints.Goals.GetById;
public class GetGoalEndpoint : Endpoint<Guid, GoalDto>
{
    private readonly IGoalQueryStore _goalQueryStore;

    public GetGoalEndpoint(IGoalQueryStore goalQueryStore)
    {
        _goalQueryStore = goalQueryStore;
    }

    public override void Configure()
    {
        Get("goals/{id}");
        Tags("Goals");
        Policies("ApiScope");
        Description(x => x.Produces<GoalDto>()
                          .ProducesProblem(StatusCodes.Status404NotFound)
                          .ProducesProblem(StatusCodes.Status401Unauthorized)
                          .WithTags("Goals"));
    }

    public async override Task HandleAsync(Guid id, CancellationToken ct)
    {
        if (!Guid.TryParse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out Guid userId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        GoalDto goal = await _goalQueryStore.GetGoalById(userId, id, ct);

        if (goal == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SendOkAsync(goal, ct);
    }
}
