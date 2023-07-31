using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Vita.Common;
using Vita.Goals.Application.Commands.Goals.Update;
using Vita.Goals.Application.Commands.Shared;

namespace Vita.Goals.Api.Endpoints.Goals.Update;
public class UpdateGoalEndpoint : Endpoint<UpdateGoalRequest, EmptyResponse>
{
    private readonly ISender _sender;
    private readonly AutoMapper.IMapper _mapper;

    public UpdateGoalEndpoint(ISender sender, AutoMapper.IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
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
        User user = _mapper.Map<User>(User);

        DateTimeInterval? aimDate = request.AimDateStart.HasValue && request.AimDateEnd.HasValue ?
                                   new DateTimeInterval(request.AimDateStart.Value, request.AimDateEnd.Value) :
                                   null;

        UpdateGoalCommand command = new(Route<Guid>("id"), request.Title, request.Description, aimDate, user);
        await _sender.Send(command, cancellationToken);

        await SendNoContentAsync(cancellationToken);
    }
}
