using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using Vita.Common;
using Vita.Goals.Api.Endpoints.Goals.GetById;
using Vita.Goals.Application.Commands.Goals.Create;
using Vita.Goals.Application.Commands.Shared;

namespace Vita.Goals.Api.Endpoints.Goals.Create;
public class CreateGoalEndpoint : Endpoint<CreateGoalRequest, EmptyResponse>
{
    private readonly AutoMapper.IMapper _mapper;
    private readonly ISender _sender;

    public CreateGoalEndpoint(ISender sender, AutoMapper.IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
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
        User user = _mapper.Map<User>(User);

        DateTimeInterval? aimDate = request.AimDateStart.HasValue && request.AimDateEnd.HasValue ?
                                    new DateTimeInterval(request.AimDateStart.Value, request.AimDateEnd.Value) :
                                    null;

        CreateGoalCommand command = new(request.Title, request.Description, user.Id, aimDate);

        Guid createdGoalId = await _sender.Send(command, ct);

        await SendCreatedAtAsync<GetGoalEndpoint>
        (
            routeValues: new { id = createdGoalId },
            responseBody: new EmptyResponse(),
            cancellation: ct
        );
    }
}
