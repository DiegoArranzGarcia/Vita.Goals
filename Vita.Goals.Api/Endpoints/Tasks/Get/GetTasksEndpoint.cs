using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MinimalApi.Endpoint;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using Vita.Goals.Application.Queries.Tasks;

namespace Vita.Goals.Api.Endpoints.Tasks.Get;
internal class GetTasksEndpoint : IEndpoint<IResult, GetTasksRequest, ClaimsPrincipal, CancellationToken>
{
    private readonly ITaskQueryStore _taskQueryStore;

    public GetTasksEndpoint(ITaskQueryStore taskQueryStore)
    {
        _taskQueryStore = taskQueryStore;
    }

    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/tasks", ([AsParameters] GetTasksRequest request, ClaimsPrincipal user, CancellationToken cancellationToken) => HandleAsync(request, user, cancellationToken))
           .Produces<IEnumerable<TaskDto>>()
           .ProducesProblem(StatusCodes.Status401Unauthorized)
           .WithMetadata(new SwaggerOperationAttribute())
           .WithTags("Tasks")
           .RequireAuthorization();
    }

    public async Task<IResult> HandleAsync(GetTasksRequest request, ClaimsPrincipal user, CancellationToken cancellationToken)
    {
        bool hasClaimUserIdClaim = Guid.TryParse(user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out Guid claimUserId);

        if (request.UserId.HasValue && hasClaimUserIdClaim && request.UserId != claimUserId)
            return Results.Unauthorized();

        IEnumerable<TaskDto> tasks = await _taskQueryStore.GetTasksCreatedByUser(request.UserId ?? claimUserId, request.Status, request.StartDate, request.EndDate, cancellationToken);

        return Results.Ok(tasks);
    }
}
