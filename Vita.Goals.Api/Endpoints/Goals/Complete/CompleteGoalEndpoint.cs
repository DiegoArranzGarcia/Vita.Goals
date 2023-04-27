using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using Vita.Goals.Application.Commands.Goals;

namespace Vita.Goals.Api.Endpoints.Goals.Complete;

internal class CompleteGoalEndpoint : Endpoint<Guid, EmptyResponse>
{
    private readonly ISender _sender;

    public CompleteGoalEndpoint(ISender sender)
    {
        _sender = sender;
    }

    public override void Configure()
    {
        Put("goals/{id:guid}/complete");
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

        CompleteGoalCommand completeGoalCommand = new(id);
        await _sender.Send(completeGoalCommand, cancellationToken);

        await SendNoContentAsync(cancellationToken);
    }
}
