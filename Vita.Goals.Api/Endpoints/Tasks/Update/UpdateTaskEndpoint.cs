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
    private readonly AutoMapper.IMapper _mapper;

    public UpdateTaskEndpoint(ISender sender, AutoMapper.IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
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
        User user = _mapper.Map<User>(User);

        UpdateTaskCommand command = new(Route<Guid>("id"), request.Title, request.GoalId, user, request.PlannedDateStart, request.PlannedDateEnd);
        await _sender.Send(command, ct);

        await SendNoContentAsync(ct);
    }
}
