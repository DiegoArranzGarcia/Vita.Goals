using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MinimalApi.Endpoint;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using Vita.Goals.Application.Commands.Tasks;

namespace Vita.Goals.Api.Endpoints.Tasks.Delete;
internal class DeleteTaskEndpoint : IEndpoint<IResult, Guid, ClaimsPrincipal, CancellationToken>
{
    private readonly ISender _sender;

    public DeleteTaskEndpoint(ISender sender)
    {
        _sender = sender;
    }

    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/tasks/{id:guid}", (Guid id, ClaimsPrincipal user, CancellationToken cancellationToken) => HandleAsync(id, user, cancellationToken))
           .Produces(StatusCodes.Status204NoContent)
           .ProducesProblem(StatusCodes.Status401Unauthorized)
           .WithMetadata(new SwaggerOperationAttribute())
           .WithTags("Tasks")
           .RequireAuthorization();
    }

    public async Task<IResult> HandleAsync(Guid id, ClaimsPrincipal user, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out Guid userId))
            return Results.Unauthorized();

        DeleteTaskCommand command = new(id);
        await _sender.Send(command, cancellationToken);

        return Results.NoContent();
    }
}
