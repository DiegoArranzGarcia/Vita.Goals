using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Vita.Goals.Application.Commands.Goals.Delete;
using Vita.Goals.Application.Commands.Shared;

namespace Vita.Goals.Api.Endpoints.Goals.Delete;
internal class DeleteGoalEndpoint : Endpoint<EmptyRequest, EmptyResponse>
{
    private readonly ISender _sender;
    private readonly AutoMapper.IMapper _mapper;

    public DeleteGoalEndpoint(ISender sender, AutoMapper.IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }

    public override void Configure()
    {
        Delete("goals/{id}");
        Policies("ApiScope");
        Description(x => x.Produces(StatusCodes.Status204NoContent)
                          .ProducesProblem(StatusCodes.Status401Unauthorized)
                          .WithTags("Goals"));
    }

    public async override Task HandleAsync(EmptyRequest request, CancellationToken ct)
    {
        User user = _mapper.Map<User>(User);

        DeleteGoalCommand deleteGoalCommand = new(Route<Guid>("id"), user);
        await _sender.Send(deleteGoalCommand, ct);

        await SendNoContentAsync(ct);
    }
}
