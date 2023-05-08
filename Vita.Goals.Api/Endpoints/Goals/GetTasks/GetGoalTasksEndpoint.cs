using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Vita.Goals.Application.Queries.Goals;

namespace Vita.Goals.Api.Endpoints.Goals.GetTasks;
internal class GetGoalTasksEndpoint : Endpoint<Guid, IEnumerable<GoalTaskDto>>
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

    public async override Task HandleAsync(Guid id, CancellationToken cancellationToken)
    {
        IEnumerable<GoalTaskDto> tasks = await _goalQueryStore.GetGoalTasks(id, cancellationToken);

        await SendOkAsync(tasks, cancellationToken);
    }
}
