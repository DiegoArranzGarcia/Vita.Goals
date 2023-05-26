using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Net;
using Vita.Goals.Application.Commands.Calendars.CreateCalendar;

namespace Vita.Goals.Api.Endpoints.Calendars.Create;

internal class CreateCalendarEndpoint : Endpoint<CreateCalendarRequest>
{
    private readonly ISender _sender;

    public CreateCalendarEndpoint(ISender sender)
    {
        _sender = sender;
    }

    public override void Configure()
    {
        Post("calendars");
        Description(x => x.Produces((int)HttpStatusCode.NoContent)
                          .WithTags("Calendars"));
    }

    public async override Task HandleAsync(CreateCalendarRequest request, CancellationToken ct)
    {
        CreateCalendarCommand command = new(request.UserId, request.ProviderName);

        await _sender.Send(command, ct);

        await SendNoContentAsync(ct);
    }
}
