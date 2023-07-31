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
    private readonly AutoMapper.IMapper _mapper;

    public CreateTaskEndpoint(ISender sender, AutoMapper.IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
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
        User user = _mapper.Map<User>(User);
    
        DateTimeInterval? plannedDate = request.PlannedDateStart.HasValue && request.PlannedDateEnd.HasValue ?
                                        new DateTimeInterval(request.PlannedDateStart.Value, request.PlannedDateEnd.Value) :
                                        null;

        CreateTaskCommand command = new(request.GoalId, request.Title, user, plannedDate);
        Guid createdTaskId = await _sender.Send(command, ct);

        await SendCreatedAtAsync<GetTaskEndpoint>
        (
            routeValues: new { id = createdTaskId },
            responseBody: new EmptyResponse(),
            cancellation: ct
        );
    }
}
