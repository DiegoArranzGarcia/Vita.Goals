using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vita.Goals.Application.Commands.Services.CalendarServiceProviders;
using Vita.Goals.Infrastructure.Clients.Identity;

namespace Vita.Goals.Application.Commands.Calendars.CreateCalendar;
public class CreateCalendarCommandHandler : IRequestHandler<CreateCalendarCommand>
{
    private readonly VitaIdentityApiClient _vitaIdentityApiClient;
    private readonly CalendarServicesProviderFactory _calendarServicesProviderFactory;

    public CreateCalendarCommandHandler(VitaIdentityApiClient vitaIdentityApiClient, CalendarServicesProviderFactory calendarServicesProviderFactory)
    {
        _vitaIdentityApiClient = vitaIdentityApiClient;
        _calendarServicesProviderFactory = calendarServicesProviderFactory;
    }

    public async Task Handle(CreateCalendarCommand request, CancellationToken cancellationToken)
    {
        IEnumerable<LoginProviderDto> loginProviders = await _vitaIdentityApiClient.GetExternalLoginProviders(request.UserId);
        LoginProviderDto loginProvider = loginProviders.FirstOrDefault(x => x.Name == request.ProviderName);

        if (loginProvider is null)
            throw new Exception("Login Provider not found");

        ICalendarServicesProvider calendarServicesProvider = _calendarServicesProviderFactory.CreateCalendarServicesProvider(loginProvider);

        await calendarServicesProvider.CreateCalendar(request.UserId);
    }
}
