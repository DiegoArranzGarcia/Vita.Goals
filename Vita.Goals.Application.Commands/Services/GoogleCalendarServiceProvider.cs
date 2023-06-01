using Google.Apis.Calendar.v3.Data;
using IdentityModel.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using Vita.Goals.Application.Commands.Services.CalendarServiceProviders;
using Vita.Goals.Domain.Aggregates.Tasks;
using Vita.Goals.Infrastructure.Clients.Identity;

namespace Vita.Goals.Application.Commands.Services;

internal class GoogleCalendarServiceProvider : ICalendarServicesProvider
{
    private readonly LoginProviderDto _loginProvider;
    private readonly VitaIdentityApiClient _vitaIdentityApiClient;
    private readonly ITaskRepository _taskRepository;
    private readonly Google.Apis.Calendar.v3.CalendarService _calendarService = new();

    public GoogleCalendarServiceProvider(LoginProviderDto loginProvider, VitaIdentityApiClient vitaIdentityApiClient, ITaskRepository taskRepository)
    {
        _loginProvider = loginProvider;
        _vitaIdentityApiClient = vitaIdentityApiClient;
        _taskRepository = taskRepository;
    }

    public async System.Threading.Tasks.Task CreateCalendar(Guid userId)
    {
        AccessTokenDto googleAccessToken = await _vitaIdentityApiClient.GetLoginProviderUserAccessToken(userId, _loginProvider.Id);
        _calendarService.HttpClient.SetBearerToken(googleAccessToken.Token);

        IEnumerable<Task> tasks = _taskRepository.Get((task) => task.AssociatedTo.CreatedBy == userId &&
                                                                                        task.TaskStatus != Domain.Aggregates.Tasks.TaskStatus.Completed);

        IEnumerable<Task> scheduledTasks = tasks.Where(x => x.PlannedDate is not null);

        Calendar tasksCalendar = await _calendarService.Calendars.Insert(new Calendar()
        {
            Summary = "Vita",
            Description = "Vita Calendar"
        }).ExecuteAsync();

        foreach (Task scheduledTask in scheduledTasks)
        {
            await _calendarService.Events.Insert(new Event()
            {
                Summary = scheduledTask.Title,
                Start = new EventDateTime() { DateTime = scheduledTask.PlannedDate.Start.DateTime },
                End = new EventDateTime() { DateTime = scheduledTask.PlannedDate.End.DateTime },
            }, tasksCalendar.Id).ExecuteAsync();
        }
    }

    public async System.Threading.Tasks.Task DeleteCalendar(Guid userId)
    {
        AccessTokenDto googleAccessToken = await _vitaIdentityApiClient.GetLoginProviderUserAccessToken(userId, _loginProvider.Id);

        _calendarService.HttpClient.SetBearerToken(googleAccessToken.Token);

        CalendarList calendarList = await _calendarService.CalendarList.List().ExecuteAsync();
        CalendarListEntry calendarListEntry = calendarList.Items.FirstOrDefault(x => x.Summary == "Vita");

        if (calendarListEntry is null)
            return;

        _calendarService.Calendars.Delete(calendarListEntry.Id);
    }
}
