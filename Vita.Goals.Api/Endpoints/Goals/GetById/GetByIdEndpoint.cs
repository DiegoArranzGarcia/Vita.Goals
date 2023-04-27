using FastEndpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;
using Vita.Goals.Application.Queries.Goals;

namespace Vita.Goals.Api.Endpoints.Goals.GetById;
internal class GetByIdEndpoint : Endpoint<Guid, GoalDto>
{
    private readonly IGoalQueryStore _goalQueryStore;

    public GetByIdEndpoint(IGoalQueryStore goalQueryStore)
    {
        _goalQueryStore = goalQueryStore;
    }

    public override void Configure()
    {
        Get("goals/{id:guid}");
        Tags("Goals");
        Policies("ApiScope");
        Description(x => x.Produces<GoalDto>()
                          .ProducesProblem(StatusCodes.Status404NotFound)
                          .ProducesProblem(StatusCodes.Status401Unauthorized)
                          .WithTags("Goals"));                                    
    }

    public async override Task HandleAsync(Guid id, CancellationToken cancellationToken)
    {
        GoalDto goal = await _goalQueryStore.GetGoalById(id, cancellationToken);

        if (goal == null)
        {
            await SendNotFoundAsync(cancellationToken);
            return;
        }

        await SendOkAsync(goal, cancellationToken);
    }
}
