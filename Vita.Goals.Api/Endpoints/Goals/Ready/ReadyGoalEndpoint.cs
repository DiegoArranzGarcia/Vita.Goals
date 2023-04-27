using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Vita.Goals.Application.Commands.Goals;

namespace Vita.Goals.Api.Endpoints.Goals.Ready;

internal class ReadyGoalEndpoint : Endpoint<Guid, EmptyResponse>
{
    private readonly ISender _sender;

    public ReadyGoalEndpoint(ISender sender)
    {
        _sender = sender;
    }

    public override void Configure()
    {
        Put("goals/{id:guid}/ready");
        Policies("ApiScope");
        Description(x => x.Produces(StatusCodes.Status204NoContent)
                          .ProducesProblem(StatusCodes.Status401Unauthorized)
                          .WithTags("Goals"));
    }

    public async override Task HandleAsync(Guid id, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out Guid userId))
        {
            await SendUnauthorizedAsync(cancellationToken);
            return;
        }

        ReadyGoalCommand command = new(id);
        await _sender.Send(command, cancellationToken);

        await SendNoContentAsync(cancellationToken);
    }
}
