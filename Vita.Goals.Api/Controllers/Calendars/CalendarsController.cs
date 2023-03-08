using MediatR;
using Microsoft.AspNetCore.Mvc;
using Vita.Goals.Application.Commands.Calendars.CreateCalendar;
using Vita.Goals.Application.Commands.Calendars.DeleteCalendar;

namespace Vita.Goals.Api.Controllers.Calendars;

[ApiController]
[Route("api/[controller]")]
public class CalendarsController : ControllerBase
{
    private readonly ISender _sender;

    public CalendarsController(ISender sender)
    {
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));
    }

    [HttpPost]
    public async Task<IActionResult> CreateCalendar(CreateCalendarDto createCalendarDto)
    {
        CreateCalendarCommand command = new(createCalendarDto.UserId, createCalendarDto.ProviderName);

        await _sender.Send(command);

        return new NoContentResult();
    }

    [HttpDelete]
    [Route("{loginProviderId}")]
    public async Task<IActionResult> DeleteCalendar(Guid loginProviderId, DeleteCalendarDto deleteCalendarDto)
    {
        DeleteCalendarCommand command = new(deleteCalendarDto.UserId, loginProviderId);

        await _sender.Send(command);

        return new NoContentResult();
    }
}
