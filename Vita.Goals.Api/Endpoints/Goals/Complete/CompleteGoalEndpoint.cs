using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using Vita.Goals.Application.Commands.Goals.Complete;
using Vita.Goals.Application.Commands.Shared;

namespace Vita.Goals.Api.Endpoints.Goals.Complete;

public class CompleteGoalEndpoint : Endpoint<EmptyRequest, EmptyResponse>
{
    private readonly ISender _sender;
    private readonly AutoMapper.IMapper _mapper;

    public CompleteGoalEndpoint(ISender sender, AutoMapper.IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }

    public override void Configure()
    {
        Put("goals/{id}/complete");
        Policies("ApiScope");
        Description(x => x.Produces(StatusCodes.Status204NoContent)
                          .ProducesProblem(StatusCodes.Status401Unauthorized)
                          .WithTags("Goals"));
        DontCatchExceptions();
    }

    public async override Task HandleAsync(EmptyRequest reñquest, CancellationToken ct)
    {
        User user = _mapper.Map<User>(User);

        CompleteGoalCommand completeGoalCommand = new(Route<Guid>("id"), user);
        await _sender.Send(completeGoalCommand, ct);

        await SendNoContentAsync(ct);
    }
}
