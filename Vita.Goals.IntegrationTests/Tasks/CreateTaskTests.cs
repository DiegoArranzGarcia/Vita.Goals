using FastEndpoints;
using FluentAssertions;
using System.Net;
using Vita.Goals.Api.Endpoints.Tasks.Create;
using Vita.Goals.Domain.Aggregates.Goals;
using Vita.Goals.FunctionalTests.Fixtures.Builders;
using Vita.Goals.FunctionalTests.Fixtures.Extensions;
using Vita.Goals.FunctionalTests.Tasks.Fixtures;
using Vita.Goals.Infrastructure.Sql;

namespace Vita.Goals.FunctionalTests.Tasks;

[Collection(nameof(TasksTestCollection))]
public class CreateTaskTests
{
    private TasksTestsFixture Given { get; }

    public CreateTaskTests(TasksTestsFixture TasksTestsFixture)
    {
        Given = TasksTestsFixture;
    }

    [Fact]
    public async Task GivenUnauthenticatedUser_WhenCreatingTask_ThenReturnsUnauthorized()
    {
        Goal goal = await Given.AGoalInTheDatabase(UserBuilder.AliceUserId);
        CreateTaskRequest request = TasksTestsFixture.BuildCreateTaskRequest(goal.Id);

        HttpClient httpClient = Given.CreateClient();

        var (response, _) = await httpClient.POSTAsync<CreateTaskEndpoint, CreateTaskRequest, EmptyResponse>(request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GivenUnauthorizedUser_WhenCreatingTask_ThenReturnsForbidden()
    {
        Goal goal = await Given.AGoalInTheDatabase(UserBuilder.AliceUserId);
        CreateTaskRequest request = TasksTestsFixture.BuildCreateTaskRequest(goal.Id);

        HttpClient httpClient = Given.CreateClient()
                                     .WithIdentity(UserBuilder.UnauthorizedUserClaims);

        var (response, _) = await httpClient.POSTAsync<CreateTaskEndpoint, CreateTaskRequest, EmptyResponse>(request);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GivenGoalOfOtherUser_WhenCreatingTask_ThenReturnsForbidden()
    {
        Goal goal = await Given.AGoalInTheDatabase(UserBuilder.BobUserId);
        CreateTaskRequest request = TasksTestsFixture.BuildCreateTaskRequest(goal.Id);

        HttpClient httpClient = Given.CreateClient()
                                     .WithIdentity(UserBuilder.AliceClaims);

        var (response, _) = await httpClient.POSTAsync<CreateTaskEndpoint, CreateTaskRequest, EmptyResponse>(request);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }


    [Fact]
    public async Task GivenUnexistentGoal_WhenCreatingTask_ThenReturnsNotFound()
    {
        CreateTaskRequest request = TasksTestsFixture.BuildCreateTaskRequest(Guid.NewGuid());

        HttpClient httpClient = Given.CreateClient()
                                     .WithIdentity(UserBuilder.AliceClaims);

        var (response, _) = await httpClient.POSTAsync<CreateTaskEndpoint, CreateTaskRequest, EmptyResponse>(request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GivenAuthorizedUser_WhenCreatingTask_ThenCreatesTheTask()
    {
        await Given.CleanDatabase();

        Goal goal = await Given.AGoalInTheDatabase(UserBuilder.AliceUserId);
        CreateTaskRequest request = TasksTestsFixture.BuildCreateTaskRequest(goal.Id);

        HttpClient httpClient = Given.CreateClient()
                                     .WithIdentity(UserBuilder.AliceClaims);

        var (response, _) = await httpClient.POSTAsync<CreateTaskEndpoint, CreateTaskRequest, EmptyResponse>(request);

        response.StatusCode.Should().Be(HttpStatusCode.Created, because: await response.Content.ReadAsStringAsync());

        GoalsDbContext context = Given.GetGoalsDbContext();
        Domain.Aggregates.Tasks.Task task = context.Tasks.Single();

        task.Title.Should().Be(request.Title);
        task.AssociatedTo.Should().BeNull();
        task.TaskStatus.Should().Be(Domain.Aggregates.Tasks.TaskStatus.Ready);
        task.PlannedDate.Should().NotBeNull();
        task.PlannedDate!.Start.Should().Be(request.PlannedDateStart);
        task.PlannedDate.End.Should().Be(request.PlannedDateEnd);
        
        //FIXME
        //Task.CreatedBy.Should().Be(UserBuilder.AliceUserId);

        response.Headers.Location.Should().Be($"/api/tasks/{task.Id}");
    }
}
