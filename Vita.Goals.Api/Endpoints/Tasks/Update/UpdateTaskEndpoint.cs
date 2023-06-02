using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Vita.Goals.Application.Commands.Shared;
using Vita.Goals.Application.Commands.Tasks.Update;

namespace Vita.Goals.Api.Endpoints.Tasks.Update;
public class UpdateTaskEndpoint : Endpoint<UpdateTaskRequest, EmptyResponse>
{
    private readonly ISender _sender;

    public UpdateTaskEndpoint(ISender sender)
    {
        _sender = sender;
    }

    public override void Configure()
    {
        Patch("tasks/{id}");
        Policies("ApiScope");
        Description(x => x.Produces(StatusCodes.Status204NoContent)
                          .ProducesProblem(StatusCodes.Status401Unauthorized)
                          .WithTags("Goals"));
    }

    public async override Task HandleAsync(UpdateTaskRequest request, CancellationToken ct)
    {
        if (!Guid.TryParse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out Guid userId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        UpdateTaskCommand command = new(Route<Guid>("id"), request.Title, request.GoalId, new User(userId), request.PlannedDateStart, request.PlannedDateEnd);
        await _sender.Send(command, ct);

        await SendNoContentAsync(ct);
    }
}
