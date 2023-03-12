using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vita.Goals.Application.Commands.Services.CalendarServiceProviders;
using Vita.Goals.Infrastructure.Clients.Identity;

namespace Vita.Goals.Application.Commands.Calendars.DeleteCalendar;
public class DeleteCalendarCommandHandler : IRequestHandler<DeleteCalendarCommand>
{
    private readonly VitaIdentityApiClient _vitaIdentityApiClient;
    private readonly CalendarServicesProviderFactory _calendarServicesProviderFactory;

    public DeleteCalendarCommandHandler(VitaIdentityApiClient vitaIdentityApiClient, CalendarServicesProviderFactory calendarServicesProviderFactory)
    {
        _vitaIdentityApiClient = vitaIdentityApiClient;
        _calendarServicesProviderFactory = calendarServicesProviderFactory;
    }

    public async Task Handle(DeleteCalendarCommand request, CancellationToken cancellationToken)
    {
        IEnumerable<LoginProviderDto> loginProviders = await _vitaIdentityApiClient.GetExternalLoginProviders(request.UserId);
        LoginProviderDto loginProvider = loginProviders.FirstOrDefault(x => x.Id != request.LoginProviderId);

        if (loginProvider is null)
            throw new Exception("Login Provider not found");

        ICalendarServicesProvider calendarServicesProvider = _calendarServicesProviderFactory.CreateCalendarServicesProvider(loginProvider);

        await calendarServicesProvider.DeleteCalendar(request.UserId);
    }
}
