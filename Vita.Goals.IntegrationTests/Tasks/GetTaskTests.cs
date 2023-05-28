using FastEndpoints;
using FluentAssertions;
using System.Net;
using System.Net.Http;
using Vita.Goals.Api.Endpoints.Goals.Ready;
using Vita.Goals.Application.Queries.Tasks;
using Vita.Goals.FunctionalTests.Fixtures.Builders;
using Vita.Goals.FunctionalTests.Fixtures.Extensions;
using Vita.Goals.FunctionalTests.Tasks.Fixtures;
using Vita.Tasks.Api.Endpoints.Tasks.GetById;

namespace Vita.Goals.FunctionalTests.Tasks;

[Collection(nameof(TasksTestCollection))]
public class GetTaskTests
{
    private TasksTestsFixture Given { get; }

    public GetTaskTests(TasksTestsFixture tasksTestsFixture)
    {
        Given = tasksTestsFixture;
    }

    [Fact]
    public async Task GivenUnauthenticatedUser_WhenGettingTask_ThenReturnsUnauthorized()
    {
        Domain.Aggregates.Tasks.Task task = await Given.ATaskInTheDatabase(UserBuilder.AliceUserId);

        HttpClient httpClient = Given.CreateClient();

        string endpointUri = IEndpoint.TestURLFor<GetTaskEndpoint>()
                                      .Replace("{id}", task.Id.ToString());

        var (response, _) = await httpClient.GETAsync<EmptyRequest, TaskDto>(endpointUri, default);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GivenUnauthorizedUser_WhenGettingTask_ThenReturnsForbidden()
    {
        Domain.Aggregates.Tasks.Task task = await Given.ATaskInTheDatabase(UserBuilder.AliceUserId);

        HttpClient httpClient = Given.CreateClient()
                                     .WithIdentity(UserBuilder.UnauthorizedUserClaims);

        string endpointUri = IEndpoint.TestURLFor<GetTaskEndpoint>()
                                      .Replace("{id}", task.Id.ToString());

        var (response, _) = await httpClient.GETAsync<EmptyRequest, TaskDto>(endpointUri, default);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GivenTaskOfOtherUser_WhenGettingTask_ThenReturnsForbidden()
    {
        await Given.CleanDatabase();
        Domain.Aggregates.Tasks.Task aliceTask = await Given.ATaskInTheDatabase(UserBuilder.AliceUserId);

        HttpClient httpClient = Given.CreateClient()
                                     .WithIdentity(UserBuilder.BobClaims);

        string endpointUri = IEndpoint.TestURLFor<GetTaskEndpoint>()
                                      .Replace("{id}", aliceTask.Id.ToString());                

        var (response, _) = await httpClient.GETAsync<EmptyRequest, TaskDto>(endpointUri, default);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GivenAuthorizedUserButUnexistentTaskWithId_WhenGettingTask_ThenReturnsNotFound()
    {
        await Given.CleanDatabase();

        HttpClient httpClient = Given.CreateClient()
                                     .WithIdentity(UserBuilder.BobClaims);

        string endpointUri = IEndpoint.TestURLFor<GetTaskEndpoint>()
                                      .Replace("{id}", Guid.NewGuid().ToString());

        var (response, problem) = await httpClient.GETAsync<EmptyRequest, Microsoft.AspNetCore.Mvc.ProblemDetails>(endpointUri, default);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound, because: response.ReasonPhrase);

        problem.Should().NotBeNull();
        problem.Status.Should().Be((int)HttpStatusCode.NotFound);
        problem.Title.Should().Be("The given key was not present in the dictionary.");
        problem.Detail.Should().NotBeNullOrEmpty();
        problem.Instance.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GivenAUserTask_WhenGettingTask_ThenReturnsOkWithTheTask()
    {
        await Given.CleanDatabase();
        Domain.Aggregates.Tasks.Task task = await Given.ATaskWithPlannedDateInTheDatabase(UserBuilder.AliceUserId);

        HttpClient httpClient = Given.CreateClient()
                                     .WithIdentity(UserBuilder.AliceClaims);

        string endpointUri = IEndpoint.TestURLFor<GetTaskEndpoint>()
                                    .Replace("{id}", task.Id.ToString());

        var (response, taskDto) = await httpClient.GETAsync<EmptyRequest, TaskDto>(endpointUri, default);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        taskDto.Should().NotBeNull();
        taskDto.Id.Should().Be(task.Id);
        taskDto.Title.Should().Be(task.Title);
        taskDto.PlannedDateStart.Should().Be(task.PlannedDate!.Start);
        taskDto.PlannedDateEnd.Should().Be(task.PlannedDate!.End);
        taskDto.Status.Should().Be(Domain.Aggregates.Tasks.TaskStatus.Ready.Name);
    }
}
