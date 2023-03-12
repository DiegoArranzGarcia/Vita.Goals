﻿using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MinimalApi.Endpoint;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using Vita.Goals.Application.Commands.Goals;

namespace Vita.Goals.Api.Endpoints.Goals.Ready;

internal class ReadyGoalEndpoint : IEndpoint<IResult, Guid, ClaimsPrincipal, CancellationToken>
{
    private readonly ISender _sender;

    public ReadyGoalEndpoint(ISender sender)
    {
        _sender = sender;
    }

    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/goals/{id:guid}/ready", (Guid id, ClaimsPrincipal user, CancellationToken cancellationToken) => HandleAsync(id, user, cancellationToken))
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

        ReadyGoalCommand readyGoalCommand = new(id);
        await _sender.Send(readyGoalCommand, cancellationToken);

        return Results.NoContent();
    }
}