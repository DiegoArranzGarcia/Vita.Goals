using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Vita.Goals.Application.Commands.Tasks;

namespace Vita.Goals.Api.Endpoints.Tasks.Delete;
internal class DeleteTaskEndpoint : Endpoint<Guid, EmptyResponse>
{
    private readonly ISender _sender;

    public DeleteTaskEndpoint(ISender sender)
    {
        _sender = sender;
    }

    public override void Configure()
    {
        Delete("tasks/{id}");
        Policies("ApiScope");
        Description(x => x.Produces(StatusCodes.Status204NoContent)
                          .ProducesProblem(StatusCodes.Status401Unauthorized)
                          .WithTags("Tasks"));
    }

    public async override Task HandleAsync(Guid id, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out Guid userId))
        {
            await SendUnauthorizedAsync(cancellationToken);
            return;
        }

        DeleteTaskCommand deleteTaskCommand = new(id);
        await _sender.Send(deleteTaskCommand, cancellationToken);

        await SendNoContentAsync(cancellationToken);
    }
}
