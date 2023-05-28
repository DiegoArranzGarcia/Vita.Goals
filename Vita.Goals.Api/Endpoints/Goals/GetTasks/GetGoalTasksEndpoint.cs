using FastEndpoints;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Vita.Goals.Application.Queries.Goals;

namespace Vita.Goals.Api.Endpoints.Goals.GetTasks;
internal class GetGoalTasksEndpoint : Endpoint<EmptyRequest, IEnumerable<GoalTaskDto>>
{
    private readonly IGoalQueryStore _goalQueryStore;

    public GetGoalTasksEndpoint(IGoalQueryStore goalQueryStore)
    {
        _goalQueryStore = goalQueryStore;
    }

    public override void Configure()
    {
        Get("goals/{id}/tasks");
        Policies("ApiScope");
        Description(x => x.Produces<IEnumerable<GoalTaskDto>>()
                          .ProducesProblem(StatusCodes.Status404NotFound)
                          .ProducesProblem(StatusCodes.Status401Unauthorized)
                          .WithTags("Goals"));
    }

    public async override Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        if (!Guid.TryParse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out Guid userId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        IEnumerable<GoalTaskDto> tasks = await _goalQueryStore.GetGoalTasks(userId, Route<Guid>("id"), ct);

        await SendOkAsync(tasks, ct);
    }
}
