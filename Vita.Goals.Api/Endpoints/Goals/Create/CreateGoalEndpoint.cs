using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Vita.Goals.Api.Endpoints.Goals.GetById;
using Vita.Goals.Application.Commands.Goals;

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

    public async override Task HandleAsync(CreateGoalRequest request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out Guid userId))
        {
            await SendUnauthorizedAsync(cancellationToken);
            return;
        }

        CreateGoalCommand command = new(request.Title, request.Description, userId, request.AimDateStart, request.AimDateEnd);

        Guid createdGoalId = await _sender.Send(command, cancellationToken);

        //HttpContext.Response.Headers.Add("Access-Control-Allow-Headers", "Location");
        //HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "Location");

        await SendCreatedAtAsync<GetGoalEndpoint>
        (
            routeValues: new { id = createdGoalId },
            responseBody: new EmptyResponse(),
            cancellation: cancellationToken
        );
    }
}
