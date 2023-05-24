using FastEndpoints;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using Vita.Goals.Api.Endpoints.Goals.Complete;
using Vita.Goals.Api.Endpoints.Goals.GetById;
using Vita.Goals.Api.Endpoints.Goals.GetTasks;
using Vita.Goals.Application.Queries.Goals;
using Vita.Goals.Domain.Aggregates.Goals;
using Vita.Goals.FunctionalTests.Fixtures.Builders;
using Vita.Goals.FunctionalTests.Fixtures.Extensions;
using Vita.Goals.FunctionalTests.Goals.Fixtures;

namespace Vita.Goals.FunctionalTests.Goals;

[Collection(nameof(GoalsTestCollection))]
public class GetGoalTasksTests
{
    private GoalsTestsFixture Given { get; }

    public GetGoalTasksTests(GoalsTestsFixture goalsTestsFixture)
    {
        Given = goalsTestsFixture;
    }

    [Fact]
    public async Task GivenUnauthenticatedUser_WhenGettingGoalTasks_ThenReturnsUnauthorized()
    {
        Goal goal = await Given.AGoalInTheDatabase(UserBuilder.AliceUserId);

        HttpClient httpClient = Given.CreateClient();

        var (response, _) = await httpClient.GETAsync<GetGoalTasksEndpoint, Guid, IEnumerable<GoalTaskDto>>(goal.Id);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GivenUnauthorizedUser_WhenGettingGoalTasks_ThenReturnsForbidden()
    {
        Goal goal = await Given.AGoalInTheDatabase(UserBuilder.AliceUserId);

        HttpClient httpClient = Given.CreateClient()
                                     .WithIdentity(UserBuilder.UnauthorizedUserClaims);

        var (response, _) = await httpClient.GETAsync<GetGoalTasksEndpoint, Guid, IEnumerable<GoalTaskDto>>(goal.Id);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GivenGoalOfOtherUser_WhenGettingGoalTasks_ThenReturnsForbidden()
    {
        await Given.CleanDatabase();
        Goal aliceGoal = await Given.AGoalInTheDatabase(UserBuilder.AliceUserId);

        HttpClient httpClient = Given.CreateClient()
                                     .WithIdentity(UserBuilder.BobClaims);

        var (response, _) = await httpClient.GETAsync<GetGoalTasksEndpoint, Guid, IEnumerable<GoalTaskDto>>(aliceGoal.Id);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GivenAuthorizedUserButUnexistentGoalWithId_WhenCompletingGoal_ThenReturnsNotFound()
    {
        await Given.CleanDatabase();

        HttpClient httpClient = Given.CreateClient()
                                     .WithIdentity(UserBuilder.BobClaims);

        var (response, problem) = await httpClient.GETAsync<GetGoalTasksEndpoint, Guid, Microsoft.AspNetCore.Mvc.ProblemDetails>(Guid.NewGuid());

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        problem.Should().NotBeNull();
        problem.Status.Should().Be((int)HttpStatusCode.NotFound);
        problem.Title.Should().Be("The given key was not present in the dictionary.");
        problem.Detail.Should().NotBeNullOrEmpty();
        problem.Instance.Should().NotBeNullOrEmpty();
    }


    [Fact]
    public async Task GivenGoalWithTaskOfAUser_WhenGettingGoalTasks_ThenReturnsOkWithTheGoalTasks()
    {
        await Given.CleanDatabase();
        Goal aliceGoal = await Given.AGoalInTheDatabase(UserBuilder.AliceUserId);
        IReadOnlyCollection<Domain.Aggregates.Tasks.Task> tasks = await Given.GoalTasksForGoalInTheDatabase(aliceGoal.Id);

        HttpClient httpClient = Given.CreateClient()
                                     .WithIdentity(UserBuilder.AliceClaims);

        var (response, goalTasksDto) = await httpClient.GETAsync<GetGoalTasksEndpoint, Guid, IEnumerable<GoalTaskDto>>(aliceGoal.Id);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        goalTasksDto.Should().NotBeNullOrEmpty()
                    .And.HaveSameCount(tasks);
    }
}
