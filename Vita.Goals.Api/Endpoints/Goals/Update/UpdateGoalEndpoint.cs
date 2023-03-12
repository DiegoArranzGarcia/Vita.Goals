using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MinimalApi.Endpoint;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using Vita.Goals.Application.Commands.Goals;

namespace Vita.Goals.Api.Endpoints.Goals.Update;
internal class UpdateGoalEndpoint : IEndpoint<IResult, Guid, UpdateGoalRequest, ClaimsPrincipal, CancellationToken>
{
    private readonly ISender _sender;

    public UpdateGoalEndpoint(ISender sender)
    {
        _sender = sender;
    }

    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/goals/{id:guid}", (Guid id, [FromBody] UpdateGoalRequest request, ClaimsPrincipal user, CancellationToken cancellationToken) => HandleAsync(id, request, user, cancellationToken))
           .Produces(StatusCodes.Status204NoContent)
           .ProducesProblem(StatusCodes.Status401Unauthorized)
           .WithMetadata(new SwaggerOperationAttribute())
           .WithTags("Goals")
           .RequireAuthorization();
    }

    public async Task<IResult> HandleAsync(Guid id, UpdateGoalRequest request, ClaimsPrincipal user, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out Guid userId))
            return Results.Unauthorized();

        InProgressGoalCommand readyGoalCommand = new(id);
        await _sender.Send(readyGoalCommand, cancellationToken);

        return Results.NoContent();
    }
}
