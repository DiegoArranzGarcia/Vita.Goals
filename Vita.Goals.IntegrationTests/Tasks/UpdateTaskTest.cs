using FastEndpoints;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using Vita.Goals.Api.Endpoints.Tasks.Update;
using Vita.Goals.Domain.Aggregates.Goals;
using Vita.Goals.FunctionalTests.Extensions;
using Vita.Goals.FunctionalTests.Fixtures.Builders;
using Vita.Goals.FunctionalTests.Fixtures.Extensions;
using Vita.Goals.FunctionalTests.Tasks.Fixtures;
using Vita.Goals.Infrastructure.Sql;

namespace Vita.Goals.FunctionalTests.Tasks;


[Collection(nameof(TasksTestCollection))]
public class UpdateTaskTests
{
    private TasksTestsFixture Given { get; }

    public UpdateTaskTests(TasksTestsFixture goalsTestsFixture)
    {
        Given = goalsTestsFixture;
    }

    [Fact]
    public async Task GivenUnauthenticatedUser_WhenToUpdatingTask_ThenReturnsUnauthorized()
    {
        Domain.Aggregates.Tasks.Task aliceTask = await Given.ATaskInTheDatabase(UserBuilder.AliceUserId);
        Goal goal = await Given.AGoalInTheDatabase(UserBuilder.AliceUserId);

        UpdateTaskRequest request = TasksTestsFixture.BuildCreateUpdateRequest(goal.Id);

        HttpClient httpClient = Given.CreateClient();

        string endpointUri = IEndpoint.TestURLFor<UpdateTaskEndpoint>()
                                      .Replace("{id}", aliceTask.Id.ToString());

        var (response, _) = await httpClient.PATCHAsync<UpdateTaskRequest, EmptyResponse>(requestUri: endpointUri, request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GivenUnauthorizedUser_WhenUpdatingTask_ThenReturnsForbidden()
    {
        Domain.Aggregates.Tasks.Task aliceTask = await Given.ATaskInTheDatabase(UserBuilder.AliceUserId);
        Goal goal = await Given.AGoalInTheDatabase(UserBuilder.AliceUserId);
        UpdateTaskRequest request = TasksTestsFixture.BuildCreateUpdateRequest(goal.Id);

        HttpClient httpClient = Given.CreateClient()
                                     .WithIdentity(UserBuilder.UnauthorizedUserClaims);

        string endpointUri = IEndpoint.TestURLFor<UpdateTaskEndpoint>()
                                      .Replace("{id}", aliceTask.Id.ToString());

        var (response, _) = await httpClient.PATCHAsync<UpdateTaskRequest, EmptyResponse>(requestUri: endpointUri, request);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }


    [Fact]
    public async Task GivenTaskOfOtherUser_WhenUpdatingTask_ThenReturnsForbidden()
    {
        Domain.Aggregates.Tasks.Task aliceTask = await Given.ATaskInTheDatabase(UserBuilder.AliceUserId);
        Goal goal = await Given.AGoalInTheDatabase(UserBuilder.AliceUserId);
        UpdateTaskRequest request = TasksTestsFixture.BuildCreateUpdateRequest(goal.Id);

        HttpClient httpClient = Given.CreateClient()
                                     .WithIdentity(UserBuilder.BobClaims);

        string endpointUri = IEndpoint.TestURLFor<UpdateTaskEndpoint>()
                                      .Replace("{id}", aliceTask.Id.ToString());

        var (response, _) = await httpClient.PATCHAsync<UpdateTaskRequest, EmptyResponse>(requestUri: endpointUri, request);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GivenAGoalOfOtherUser_WhenUpdatingTask_ThenReturnsForbidden()
    {
        Domain.Aggregates.Tasks.Task aliceTask = await Given.ATaskInTheDatabase(UserBuilder.AliceUserId);
        Goal goal = await Given.AGoalInTheDatabase(UserBuilder.BobUserId);
        UpdateTaskRequest request = TasksTestsFixture.BuildCreateUpdateRequest(goal.Id);

        HttpClient httpClient = Given.CreateClient()
                                     .WithIdentity(UserBuilder.BobClaims);

        string endpointUri = IEndpoint.TestURLFor<UpdateTaskEndpoint>()
                                      .Replace("{id}", aliceTask.Id.ToString());

        var (response, _) = await httpClient.PATCHAsync<UpdateTaskRequest, EmptyResponse>(requestUri: endpointUri, request);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GivenAuthorizedUserButUnexistentTaskWithId_WhenUpdatingTask_ThenReturnsNotFound()
    {
        Goal goal = await Given.AGoalInTheDatabase(UserBuilder.AliceUserId);
        UpdateTaskRequest request = TasksTestsFixture.BuildCreateUpdateRequest(goal.Id);

        HttpClient httpClient = Given.CreateClient()
                                     .WithIdentity(UserBuilder.BobClaims);

        string endpointUri = IEndpoint.TestURLFor<UpdateTaskEndpoint>()
                                      .Replace("{id}", Guid.NewGuid().ToString());

        var (response, _) = await httpClient.PATCHAsync<UpdateTaskRequest, EmptyResponse>(requestUri: endpointUri, request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        Microsoft.AspNetCore.Mvc.ProblemDetails problem = (await response.Content.ReadFromJsonAsync<Microsoft.AspNetCore.Mvc.ProblemDetails>())!;

        problem.Should().NotBeNull();
        problem.Status.Should().Be((int)HttpStatusCode.NotFound);
        problem.Title.Should().Be("The given key was not present in the dictionary.");
        problem.Detail.Should().NotBeNullOrEmpty();
        problem.Instance.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GivenAuthorizedUserAndExistentTaskId_WhenUpdatingTask_ThenReturnsNoContent_And_UpdatesTheTaskStastusToUpdate()
    {
        Domain.Aggregates.Tasks.Task aliceTask = await Given.ATaskInTheDatabase(UserBuilder.AliceUserId);
        Goal goal = await Given.AGoalInTheDatabase(UserBuilder.AliceUserId);
        UpdateTaskRequest request = TasksTestsFixture.BuildCreateUpdateRequest(goal.Id);

        HttpClient httpClient = Given.CreateClient()
                                     .WithIdentity(UserBuilder.AliceClaims);

        string endpointUri = IEndpoint.TestURLFor<UpdateTaskEndpoint>()
                                      .Replace("{id}", aliceTask.Id.ToString());

        var (response, _) = await httpClient.PATCHAsync<UpdateTaskRequest, EmptyResponse>(requestUri: endpointUri, request);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        GoalsDbContext context = Given.GetGoalsDbContext();
        Domain.Aggregates.Tasks.Task updatedTask = context.Tasks.First(x => x.Id == aliceTask.Id);

        updatedTask.Title.Should().Be(request.Title);
        updatedTask.TaskStatus.Should().Be(aliceTask.TaskStatus);
        updatedTask.PlannedDate.Should().NotBeNull();
        updatedTask.PlannedDate!.Start.Should().Be(request.PlannedDateStart);
        updatedTask.PlannedDate!.End.Should().Be(request.PlannedDateEnd);
    }
}