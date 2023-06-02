using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Vita.Common;
using Vita.Goals.Api.Endpoints.Tasks.GetById;
using Vita.Goals.Application.Commands.Shared;
using Vita.Goals.Application.Commands.Tasks.Create;

namespace Vita.Goals.Api.Endpoints.Tasks.Create;
public class CreateTaskEndpoint : Endpoint<CreateTaskRequest, EmptyResponse>
{
    private readonly ISender _sender;

    public CreateTaskEndpoint(ISender sender)
    {
        _sender = sender;
    }

    public override void Configure()
    {
        Post("tasks");
        Policies("ApiScope");
        Description(x => x.Produces(StatusCodes.Status201Created)
                          .ProducesProblem(StatusCodes.Status401Unauthorized)
                          .WithTags("Tasks"));
    }

    public async override Task HandleAsync(CreateTaskRequest request, CancellationToken ct)
    {
        if (!Guid.TryParse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out Guid userId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        DateTimeInterval? plannedDate = request.PlannedDateStart.HasValue && request.PlannedDateEnd.HasValue ?
                                        new DateTimeInterval(request.PlannedDateStart.Value, request.PlannedDateEnd.Value) :
                                        null;

        CreateTaskCommand command = new(request.GoalId, request.Title, new User(userId), plannedDate);
        Guid createdTaskId = await _sender.Send(command, ct);

        await SendCreatedAtAsync<GetTaskEndpoint>
        (
            routeValues: new { id = createdTaskId },
            responseBody: new EmptyResponse(),
            cancellation: ct
        );
    }
}
