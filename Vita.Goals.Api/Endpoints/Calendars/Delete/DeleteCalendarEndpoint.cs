using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MinimalApi.Endpoint;
using Swashbuckle.AspNetCore.Annotations;
using Vita.Goals.Application.Commands.Calendars.DeleteCalendar;

namespace Vita.Goals.Api.Endpoints.Calendars.Delete;

internal class DeleteCalendarEndpoint : IEndpoint<IResult, Guid, DeleteCalendarRequest, CancellationToken>
{
    private readonly ISender _sender;

    public DeleteCalendarEndpoint(ISender sender)
    {
        _sender = sender;
    }

    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/calendars/{loginProviderId}", (Guid loginProviderId, [FromBody] DeleteCalendarRequest request, CancellationToken cancellationToken) => HandleAsync(loginProviderId, request, cancellationToken))
           .Produces(StatusCodes.Status204NoContent)
           .WithMetadata(new SwaggerOperationAttribute())
           .WithTags("Calendars");
    }

    public async Task<IResult> HandleAsync(Guid logiunProvider, DeleteCalendarRequest request, CancellationToken cancellationToken)
    {
        DeleteCalendarCommand command = new(request.UserId, logiunProvider);

        await _sender.Send(command, cancellationToken);

        return Results.NoContent();
    }
}
