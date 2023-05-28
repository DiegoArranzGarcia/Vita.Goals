using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Vita.Goals.Application.Commands.Shared;
using Vita.Goals.Application.Commands.Tasks;
using Vita.Tasks.Api.Endpoints.Tasks.GetById;

namespace Vita.Goals.Api.Endpoints.Tasks.Create;
internal class CreateTaskEndpoint : Endpoint<CreateTaskRequest, EmptyResponse>
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

        CreateTaskCommand command = new(request.GoalId, request.Title, new User(userId), request.PlannedDateStart, request.PlannedDateEnd);
        Guid createdTaskId = await _sender.Send(command, ct);
   
        await SendCreatedAtAsync<GetTaskEndpoint>
        (
            routeValues: new { id = createdTaskId },
            responseBody: new EmptyResponse(),
            cancellation: ct
        );
    }
}
