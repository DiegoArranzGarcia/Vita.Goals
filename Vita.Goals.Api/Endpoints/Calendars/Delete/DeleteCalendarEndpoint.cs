using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Vita.Goals.Application.Commands.Calendars.DeleteCalendar;

namespace Vita.Goals.Api.Endpoints.Calendars.Delete;

internal class DeleteCalendarEndpoint : Endpoint<DeleteCalendarRequest, EmptyResult>
{
    private readonly ISender _sender;

    public DeleteCalendarEndpoint(ISender sender)
    {
        _sender = sender;
    }

    public override void Configure()
    {
        Delete("calendars/{loginProviderId}");
        Description(x => x.Produces(StatusCodes.Status204NoContent)
                          .WithTags("Calendars"));
    }

    public async override Task HandleAsync(DeleteCalendarRequest request, CancellationToken ct)
    {
        if (!Guid.TryParse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out Guid userId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        DeleteCalendarCommand command = new(userId, Route<Guid>("loginProvider"));

        await _sender.Send(command, ct);

        await SendNoContentAsync(ct);
    }
}
