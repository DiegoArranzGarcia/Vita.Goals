using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MinimalApi.Endpoint;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using Vita.Goals.Application.Commands.Tasks;

namespace Vita.Goals.Api.Endpoints.Tasks.Update;
internal class UpdateTaskEndpoint : IEndpoint<IResult, Guid, UpdateTaskRequest, ClaimsPrincipal, CancellationToken>
{
    private readonly ISender _sender;

    public UpdateTaskEndpoint(ISender sender)
    {
        _sender = sender;
    }

    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPatch("/api/tasks/{id:guid}", (Guid id, [FromBody] UpdateTaskRequest request, ClaimsPrincipal user, CancellationToken cancellationToken) => HandleAsync(id, request, user, cancellationToken))
           .Produces(StatusCodes.Status204NoContent)
           .ProducesProblem(StatusCodes.Status401Unauthorized)
           .WithMetadata(new SwaggerOperationAttribute())
           .WithTags("Tasks")
           .RequireAuthorization();
    }

    public async Task<IResult> HandleAsync(Guid id, UpdateTaskRequest request, ClaimsPrincipal user, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out Guid userId))
            return Results.Unauthorized();

        UpdateTaskCommand command = new(id, request.Title, request.GoalId, request.PlannedDateEnd, request.PlannedDateStart);
        await _sender.Send(command, cancellationToken);

        return Results.NoContent();
    }
}
