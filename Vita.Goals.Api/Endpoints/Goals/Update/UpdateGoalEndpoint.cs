using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Vita.Goals.Application.Commands.Goals;
using Vita.Goals.Application.Commands.Shared;

namespace Vita.Goals.Api.Endpoints.Goals.Update;
internal class UpdateGoalEndpoint : Endpoint<UpdateGoalRequest, EmptyResponse>
{
    private readonly ISender _sender;

    public UpdateGoalEndpoint(ISender sender)
    {
        _sender = sender;
    }

    public override void Configure()
    {
        Patch("goals/{id}");
        Policies("ApiScope");
        Description(x => x.Produces(StatusCodes.Status204NoContent)
                          .ProducesProblem(StatusCodes.Status401Unauthorized)
                          .WithTags("Goals"));
    }

    public async override Task HandleAsync(UpdateGoalRequest request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out Guid userId))
        {
            await SendUnauthorizedAsync(cancellationToken);
            return;
        }

        UpdateGoalCommand command = new(Route<Guid>("id"), request.Title, request.Description, request.AimDateStart, request.AimDateEnd, new User(userId));
        await _sender.Send(command, cancellationToken);

        await SendNoContentAsync(cancellationToken);
    }
}
