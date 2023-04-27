using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Vita.Goals.Application.Commands.Goals;

namespace Vita.Goals.Api.Endpoints.Goals.InProgress;

internal class InProgressGoalEndpoint : Endpoint<Guid, EmptyResponse>
{
    private readonly ISender _sender;

    public InProgressGoalEndpoint(ISender sender)
    {
        _sender = sender;
    }

    public override void Configure()
    {
        Put("goals/{id:guid}/in-progress");
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

        InProgressGoalCommand command = new(id);
        await _sender.Send(command, cancellationToken);

        await SendNoContentAsync(cancellationToken);
    }
}
