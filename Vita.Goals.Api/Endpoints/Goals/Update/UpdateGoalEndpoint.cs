using FastEndpoints;
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
internal class UpdateGoalEndpoint : Endpoint<UpdateGoalRequest, EmptyResponse>
{
    private readonly ISender _sender;

    public UpdateGoalEndpoint(ISender sender)
    {
        _sender = sender;
    }

    public override void Configure()
    {
        Put("goals/{id:guid}");
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

        UpdateGoalRequest command = new(request.Title, request.Description, request.AimDateStart, request.AimDateEnd);
        await _sender.Send(command, cancellationToken);

        await SendNoContentAsync(cancellationToken);
    }
}
