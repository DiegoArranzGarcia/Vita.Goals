using FastEndpoints;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using Vita.Goals.Api.Endpoints.Tasks.Delete;
using Vita.Goals.FunctionalTests.Fixtures.Builders;
using Vita.Goals.FunctionalTests.Fixtures.Extensions;
using Vita.Goals.FunctionalTests.Tasks.Fixtures;

namespace Vita.Goals.FunctionalTests.Tasks;

[Collection(nameof(TasksTestCollection))]
public class DeleteTaskTests
{
    private TasksTestsFixture Given { get; }

    public DeleteTaskTests(TasksTestsFixture tasksTestsFixture)
    {
        Given = tasksTestsFixture;
    }

    [Fact]
    public async Task GivenUnauthenticatedUser_WhenDeletingTask_ThenReturnsUnauthorized()
    {
        Domain.Aggregates.Tasks.Task task = await Given.ATaskInTheDatabase(UserBuilder.AliceUserId);

        HttpClient httpClient = Given.CreateClient();

        var (response, _) = await httpClient.DELETEAsync<DeleteTaskEndpoint, Guid, EmptyResponse>(task.Id);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GivenUnauthorizedUser_WhenDeletingTask_ThenReturnsForbidden()
    {
        Domain.Aggregates.Tasks.Task task = await Given.ATaskInTheDatabase(UserBuilder.AliceUserId);

        HttpClient httpClient = Given.CreateClient()
                                     .WithIdentity(UserBuilder.UnauthorizedUserClaims);

        var (response, _) = await httpClient.DELETEAsync<DeleteTaskEndpoint, Guid, EmptyResponse>(task.Id);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }


    [Fact]
    public async Task GivenAUserTask_WhenDeletingTask_ThenReturnsNoContent()
    {
        Domain.Aggregates.Tasks.Task task = await Given.ATaskInTheDatabase(UserBuilder.AliceUserId);

        HttpClient httpClient = Given.CreateClient()
                                     .WithIdentity(UserBuilder.AliceClaims);

        var (response, _) = await httpClient.DELETEAsync<DeleteTaskEndpoint, Guid, EmptyResponse>(task.Id);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task GivenANoExistentUserTask_WhenDeletingTask_ThenReturnsNotFound()
    {
        HttpClient httpClient = Given.CreateClient()
                                     .WithIdentity(UserBuilder.AliceClaims);

        var (response, _) = await httpClient.DELETEAsync<DeleteTaskEndpoint, Guid, EmptyResponse>(Guid.NewGuid());

        response.StatusCode.Should().Be(HttpStatusCode.NotFound, because: await response.Content.ReadAsStringAsync());
        Microsoft.AspNetCore.Mvc.ProblemDetails problem = (await response.Content.ReadFromJsonAsync<Microsoft.AspNetCore.Mvc.ProblemDetails>())!;

        problem.Status.Should().Be((int)HttpStatusCode.NotFound);
        problem.Title.Should().Be("The given key was not present in the dictionary.");
        problem.Detail.Should().NotBeNullOrEmpty();
        problem.Instance.Should().NotBeNullOrEmpty();
    }
}
