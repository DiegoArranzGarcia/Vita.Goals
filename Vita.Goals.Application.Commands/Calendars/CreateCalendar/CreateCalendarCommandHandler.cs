using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vita.Goals.Application.Commands.Services.CalendarServiceProviders;
using Vita.Goals.Infrastructure.Clients.Identity;

namespace Vita.Goals.Application.Commands.Calendars.CreateCalendar;
public class CreateCalendarCommandHandler : IRequestHandler<CreateCalendarCommand>
{
    private readonly IVitaIdentityApiClient _vitaIdentityApiClient;
    private readonly ICalendarServicesProviderFactory _calendarServicesProviderFactory;

    public CreateCalendarCommandHandler(IVitaIdentityApiClient vitaIdentityApiClient, ICalendarServicesProviderFactory calendarServicesProviderFactory)
    {
        _vitaIdentityApiClient = vitaIdentityApiClient;
        _calendarServicesProviderFactory = calendarServicesProviderFactory;
    }

    public async Task Handle(CreateCalendarCommand request, CancellationToken cancellationToken)
    {
        IEnumerable<LoginProviderDto> loginProviders = await _vitaIdentityApiClient.GetExternalLoginProviders(request.UserId);
        LoginProviderDto loginProvider = loginProviders.FirstOrDefault(x => x.Name == request.ProviderName) ?? throw new KeyNotFoundException("Login Provider not found");

        ICalendarServicesProvider calendarServicesProvider = _calendarServicesProviderFactory.CreateCalendarServicesProvider(loginProvider);

        await calendarServicesProvider.CreateCalendar(request.UserId);
    }
}
