using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Vita.Goals.Application.Commands.Goals.InProgress;
using Vita.Goals.Application.Commands.Shared;

namespace Vita.Goals.Api.Endpoints.Goals.InProgress;

internal class InProgressGoalEndpoint : Endpoint<EmptyRequest, EmptyResponse>
{
    private readonly ISender _sender;
    private readonly AutoMapper.IMapper _mapper;

    public InProgressGoalEndpoint(ISender sender, AutoMapper.IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }

    public override void Configure()
    {
        Put("goals/{id}/in-progress");
        Policies("ApiScope");
        Description(x => x.Produces(StatusCodes.Status204NoContent)
                          .ProducesProblem(StatusCodes.Status401Unauthorized)
                          .WithTags("Goals"));
    }

    public async override Task HandleAsync(EmptyRequest request, CancellationToken ct)
    {
        User user = _mapper.Map<User>(User);

        InProgressGoalCommand command = new(Route<Guid>("id"), user);
        await _sender.Send(command, ct);

        await SendNoContentAsync(ct);
    }
}
