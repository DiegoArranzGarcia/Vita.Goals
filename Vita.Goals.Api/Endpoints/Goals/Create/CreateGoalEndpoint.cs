﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MinimalApi.Endpoint;
using System.Security.Claims;
using Vita.Goals.Application.Commands.Goals;
using Microsoft.AspNetCore.Builder;
using Swashbuckle.AspNetCore.Annotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using FastEndpoints;

namespace Vita.Goals.Api.Endpoints.Goals.Create;
internal class CreateGoalEndpoint : Endpoint<CreateGoalRequest, EmptyResponse>
{
    private readonly ISender _sender;

    public CreateGoalEndpoint(ISender sender)
    {
        _sender = sender;
    }

    public override void Configure()
    {
        Post("goals");
        Policies("ApiScope");
        Description(x => x.Produces(StatusCodes.Status201Created)
                          .ProducesProblem(StatusCodes.Status401Unauthorized)
                          .WithTags("Goals"));
    }

    public async override Task HandleAsync(CreateGoalRequest request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out Guid userId))
        {
            await SendUnauthorizedAsync(cancellationToken);
            return;
        }

        CreateGoalCommand command = new(request.Title, request.Description, userId, request.AimDateStart, request.AimDateEnd);

        Guid createdGoalId = await _sender.Send(command, cancellationToken);

        //Response.Headers.Add("Access-Control-Allow-Headers", "Location");
        //Response.Headers.Add("Access-Control-Expose-Headers", "Location");

        await SendCreatedAtAsync(endpointName: $"api/goals/{createdGoalId}", routeValues: new { id = createdGoalId }, responseBody: new EmptyResponse(), cancellation: cancellationToken);
    }
}
