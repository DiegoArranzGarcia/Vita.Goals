using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MinimalApi.Endpoint;
using System.Security.Claims;
using Vita.Goals.Application.Commands.Goals;
using Microsoft.AspNetCore.Builder;
using Swashbuckle.AspNetCore.Annotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Vita.Goals.Api.Endpoints.Goals.Create;
internal class CreateGoalEndpoint : IEndpoint<IResult, CreateGoalRequest, ClaimsPrincipal, CancellationToken>
{
    private readonly ISender _sender;

    public CreateGoalEndpoint(ISender sender)
    {
        _sender = sender;
    }

    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/goals", ([FromBody] CreateGoalRequest request, ClaimsPrincipal user, CancellationToken cancellationToken) => HandleAsync(request, user, cancellationToken))
           .Produces(StatusCodes.Status201Created)
           .ProducesProblem(StatusCodes.Status401Unauthorized)
           .WithMetadata(new SwaggerOperationAttribute())
           .WithTags("Goals")
           .RequireAuthorization();
    }

    public async Task<IResult> HandleAsync(CreateGoalRequest request, ClaimsPrincipal user, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out Guid userId))
            return Results.Unauthorized();

        CreateGoalCommand command = new(request.Title, request.Description, userId, request.AimDateStart, request.AimDateEnd);

        Guid createdGoalId = await _sender.Send(command, cancellationToken);

        //Response.Headers.Add("Access-Control-Allow-Headers", "Location");
        //Response.Headers.Add("Access-Control-Expose-Headers", "Location");

        return Results.CreatedAtRoute(routeName: $"api/goals/{createdGoalId}", routeValues: new { id = createdGoalId }, value: null);
    }
}
