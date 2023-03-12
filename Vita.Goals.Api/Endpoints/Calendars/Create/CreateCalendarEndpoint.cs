using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MinimalApi.Endpoint;
using Swashbuckle.AspNetCore.Annotations;
using Vita.Goals.Application.Commands.Calendars.CreateCalendar;

namespace Vita.Goals.Api.Endpoints.Calendars.Create;

internal class CreateCalendarEndpoint : IEndpoint<IResult, CreateCalendarRequest>
{
    private readonly ISender _sender;

    public CreateCalendarEndpoint(ISender sender)
    {
        _sender = sender;
    }

    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/calendars", ([FromBody] CreateCalendarRequest request) => HandleAsync(request))
           .Produces(StatusCodes.Status204NoContent)
           .WithMetadata(new SwaggerOperationAttribute())
           .WithTags("Calendars");
    }

    public async Task<IResult> HandleAsync(CreateCalendarRequest request)
    {
        CreateCalendarCommand command = new(request.UserId, request.ProviderName);

        await _sender.Send(command);

        return Results.NoContent();
    }
}
