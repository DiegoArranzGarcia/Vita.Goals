using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Vita.Common;
using Vita.Goals.Api.Endpoints.Goals.GetById;
using Vita.Goals.Application.Commands.Goals.Create;
using Vita.Goals.Domain.ValueObjects;

namespace Vita.Goals.Api.Endpoints.Goals.Create;
public class CreateGoalEndpoint : Endpoint<CreateGoalRequest, EmptyResponse>
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

    public async override Task HandleAsync(CreateGoalRequest request, CancellationToken ct)
    {
        if (!Guid.TryParse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out Guid userId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        DateTimeInterval? aimDate = request.AimDateStart.HasValue && request.AimDateEnd.HasValue ?
                                    new DateTimeInterval(request.AimDateStart.Value, request.AimDateEnd.Value) :
                                    null;

        CreateGoalCommand command = new(request.Title, request.Description, userId, aimDate);

        Guid createdGoalId = await _sender.Send(command, ct);

        await SendCreatedAtAsync<GetGoalEndpoint>
        (
            routeValues: new { id = createdGoalId },
            responseBody: new EmptyResponse(),
            cancellation: ct
        );
    }
}
