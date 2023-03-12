using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MinimalApi.Endpoint;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using Vita.Goals.Application.Commands.Goals;

namespace Vita.Goals.Api.Endpoints.Goals.Delete;
internal class DeleteGoalEndpoint : IEndpoint<IResult, Guid, ClaimsPrincipal, CancellationToken>
{
    private readonly ISender _sender;

    public DeleteGoalEndpoint(ISender sender)
    {
        _sender = sender;
    }

    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/goals/{id:guid}", (Guid id, ClaimsPrincipal user, CancellationToken cancellationToken) => HandleAsync(id, user, cancellationToken))
           .Produces(StatusCodes.Status204NoContent)
           .ProducesProblem(StatusCodes.Status401Unauthorized)
           .WithMetadata(new SwaggerOperationAttribute())
           .WithTags("Goals")
           .RequireAuthorization();
    }

    public async Task<IResult> HandleAsync(Guid id, ClaimsPrincipal user, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out Guid userId))
            return Results.Unauthorized();

        DeleteGoalCommand deleteGoalCommand = new(id);
        await _sender.Send(deleteGoalCommand, cancellationToken);

        return Results.NoContent();
    }
}
