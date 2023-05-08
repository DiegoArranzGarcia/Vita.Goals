using FastEndpoints;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Vita.Goals.Application.Queries.Goals;

namespace Vita.Goals.Api.Endpoints.Goals.Get;
internal class GetGoalsEndpoint : Endpoint<GetGoalsRequest, IEnumerable<GoalDto>>
{
    private readonly IGoalQueryStore _goalQueryStore;

    public GetGoalsEndpoint(IGoalQueryStore goalQueryStore)
    {
        _goalQueryStore = goalQueryStore;
    }

    public override void Configure()
    {
        Get("goals");
        Policies("ApiScope");
        Description(x => x.Produces<IEnumerable<GoalDto>>()
                          .ProducesProblem(StatusCodes.Status401Unauthorized)
                          .WithTags("Goals"));
    }

    public async override Task HandleAsync(GetGoalsRequest request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out Guid userId))
        {
            await SendUnauthorizedAsync(cancellationToken);
            return;
        }

        IEnumerable<GoalDto> goals = await _goalQueryStore.GetGoalsCreatedByUser(userId, request.StartDate, request.EndDate, cancellationToken);

        await SendOkAsync(goals, cancellationToken);
    }
}
