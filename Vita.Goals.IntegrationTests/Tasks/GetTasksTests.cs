using FastEndpoints;
using FluentAssertions;
using System.Net;
using Vita.Goals.Api.Endpoints.Tasks.Get;
using Vita.Goals.Application.Queries.Tasks;
using Vita.Goals.Domain.Aggregates.Goals;
using Vita.Goals.FunctionalTests.Fixtures.Builders;
using Vita.Goals.FunctionalTests.Fixtures.Extensions;
using Vita.Goals.FunctionalTests.Tasks.Fixture;

namespace Vita.Goals.FunctionalTests.Tasks;

[Collection(nameof(TasksTestCollection))]
public class GetTasksTests
{
    private TasksTestsFixture Given { get; }

    public GetTasksTests(TasksTestsFixture tasksTestsFixture)
    {
        Given = tasksTestsFixture;
    }

    [Fact]
    public async Task GivenUnauthenticatedUser_WhenGettingAllTasks_ThenReturnsUnauthorized()
    {
        HttpClient httpClient = Given.CreateClient();

        (HttpResponseMessage response, _) =
          await httpClient.GETAsync<GetTasksEndpoint, GetTasksRequest, IEnumerable<TaskDto>>(new GetTasksRequest());

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GivenUnauthorizedUser_WhenGettingAllTasks_ThenReturnsForbidden()
    {
        HttpClient httpClient = Given.CreateClient().WithIdentity(UserBuilder.UnauthorizedUserClaims);

        (HttpResponseMessage response, _) =
          await httpClient.GETAsync<GetTasksEndpoint, GetTasksRequest, IEnumerable<TaskDto>>(new GetTasksRequest());

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GivenAuthorizedUser_WhenGettingAllTasks_ThenReturnsOkWithAllTasks()
    {
        await Given.CleanDatabase();

        IReadOnlyCollection<Domain.Aggregates.Tasks.Task> databaseTasks = await Given.TasksInTheDatabase(UserBuilder.AliceUserId);

        HttpClient httpClient = Given.CreateClient().WithIdentity(UserBuilder.AliceClaims);

        (HttpResponseMessage response, IEnumerable<TaskDto>? tasks) =
            await httpClient.GETAsync<GetTasksEndpoint, GetTasksRequest, IEnumerable<TaskDto>>(new GetTasksRequest());

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        tasks.Should().NotBeNull()
                  .And.NotBeEmpty()
                  .And.HaveSameCount(databaseTasks);
    }

    [Theory]
    [InlineData(0, 0, true)]
    [InlineData(1, 0, false)]
    [InlineData(0, 1, false)]
    public async Task GivenATaskWithStartEndDates_WhenGettingTasksWithoutShowCompletedFilter_ThenDoesntReturnTheTask(
        int endDatePlusDays,
        int startDateLessDays,
        bool shouldBeRetreived)
    {
        await Given.CleanDatabase();

        Domain.Aggregates.Tasks.Task task = await Given.ATaskWithPlannedDateInTheDatabase(UserBuilder.AliceUserId);

        HttpClient httpClient = Given.CreateClient().WithIdentity(UserBuilder.AliceClaims);
        GetTasksRequest request = new
        (

            StartDate: task.PlannedDate.End.AddDays(endDatePlusDays),
            EndDate: task.PlannedDate.Start.AddDays(-startDateLessDays)
        );

        (HttpResponseMessage response, IEnumerable<TaskDto>? tasks) =
            await httpClient.GETAsync<GetTasksEndpoint, GetTasksRequest, IEnumerable<TaskDto>>(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        tasks.Should().NotBeNull();
        tasks.Should().HaveCount(shouldBeRetreived ? 1 : 0);
    }

    [Fact]
    public async Task GivenAuthorizedUserAndTasksForOtherUser_WhenGettingAllTasks_ThenReturnsOkWithNoTasks()
    {
        await Given.CleanDatabase();
        _ = Given.TasksInTheDatabase(UserBuilder.AliceUserId);

        HttpClient httpClient = Given.CreateClient().WithIdentity(UserBuilder.BobClaims);

        (HttpResponseMessage response, IEnumerable<TaskDto>? tasks) =
            await httpClient.GETAsync<GetTasksEndpoint, GetTasksRequest, IEnumerable<TaskDto>>(new GetTasksRequest());

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        tasks.Should().NotBeNull().And.BeEmpty();
    }
}
