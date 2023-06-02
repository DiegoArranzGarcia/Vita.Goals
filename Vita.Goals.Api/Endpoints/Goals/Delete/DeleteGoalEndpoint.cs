﻿using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Vita.Goals.Application.Commands.Goals.Delete;

namespace Vita.Goals.Api.Endpoints.Goals.Delete;
internal class DeleteGoalEndpoint : Endpoint<EmptyRequest, EmptyResponse>
{
    private readonly ISender _sender;

    public DeleteGoalEndpoint(ISender sender)
    {
        _sender = sender;
    }

    public override void Configure()
    {
        Delete("goals/{id}");
        Policies("ApiScope");
        Description(x => x.Produces(StatusCodes.Status204NoContent)
                          .ProducesProblem(StatusCodes.Status401Unauthorized)
                          .WithTags("Goals"));
    }

    public async override Task HandleAsync(EmptyRequest request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out Guid userId))
        {
            await SendUnauthorizedAsync(cancellationToken);
            return;
        }

        DeleteGoalCommand deleteGoalCommand = new(Route<Guid>("id"));
        await _sender.Send(deleteGoalCommand, cancellationToken);

        await SendNoContentAsync(cancellationToken);
    }
}
