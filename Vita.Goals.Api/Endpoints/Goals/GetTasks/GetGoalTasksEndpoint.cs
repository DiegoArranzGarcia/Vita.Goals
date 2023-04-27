using FastEndpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MinimalApi.Endpoint;
using Swashbuckle.AspNetCore.Annotations;
using Vita.Goals.Application.Queries.Goals;
using Vita.Goals.Application.Queries.Tasks;
using Vita.Goals.Domain.Aggregates.Goals;

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
        Get("goals/{id:guid}/tasks");
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
