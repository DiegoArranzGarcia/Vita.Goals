using FastEndpoints;
using FluentAssertions;
using System.Net;
using Vita.Goals.Api.Endpoints.Goals.Ready;
using Vita.Goals.Domain.Aggregates.Goals;
using Vita.Goals.FunctionalTests.Fixtures.Builders;
using Vita.Goals.FunctionalTests.Fixtures.Extensions;
using Vita.Goals.Infrastructure.Sql;

namespace Vita.Goals.FunctionalTests.Goals;

[Collection(nameof(GoalsTestCollection))]
public class ReadyGoalTests
{
    private GoalsTestsFixture Given { get; }

    public ReadyGoalTests(GoalsTestsFixture goalsTestsFixture)
    {
        Given = goalsTestsFixture;
    }

    [Fact]
    public async Task GivenUnauthenticatedUser_WhenUpdatingStatusToReadyGoal_ThenReturnsUnauthorized()
    {
        Goal aliceGoal = await Given.AGoalInTheDatabase(UserBuilder.AliceUserId);

        HttpClient httpClient = Given.CreateClient();

        var (response, _) = await httpClient.PUTAsync<ReadyGoalEndpoint, Guid, EmptyResponse>(aliceGoal.Id);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GivenUnauthorizedUser_WhenUpdatingStatusToReadyGoal_ThenReturnsForbidden()
    {
        Goal aliceGoal = await Given.AGoalInTheDatabase(UserBuilder.AliceUserId);

        HttpClient httpClient = Given.CreateClient()
                                     .WithIdentity(UserBuilder.UnauthorizedUserClaims);

        var (response, _) = await httpClient.PUTAsync<ReadyGoalEndpoint, Guid, EmptyResponse>(aliceGoal.Id);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }


    [Fact]
    public async Task GivenGoalOfOtherUser_WhenUpdatingStatusToReadyGoal_ThenReturnsForbidden()
    {
        Goal aliceGoal = await Given.AGoalInTheDatabase(UserBuilder.AliceUserId);

        HttpClient httpClient = Given.CreateClient()
                                     .WithIdentity(UserBuilder.BobClaims);

        var (response, _) = await httpClient.PUTAsync<ReadyGoalEndpoint, Guid, EmptyResponse>(aliceGoal.Id);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GivenAuthorizedUserButUnexistentGoalWithId_WhenUpdatingStatusToReadyGoal_ThenReturnsNotFound()
    {
        HttpClient httpClient = Given.CreateClient()
                                     .WithIdentity(UserBuilder.BobClaims);

        var (response, _) = await httpClient.PUTAsync<ReadyGoalEndpoint, Guid, EmptyResponse>(Guid.NewGuid());

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        Microsoft.AspNetCore.Mvc.ProblemDetails problem = (await response.Content.ReadFromJsonAsync<Microsoft.AspNetCore.Mvc.ProblemDetails>())!;

        problem.Should().NotBeNull();
        problem.Status.Should().Be((int)HttpStatusCode.NotFound);
        problem.Title.Should().Be("The given key was not present in the dictionary.");
        problem.Detail.Should().NotBeNullOrEmpty();
        problem.Instance.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GivenAuthorizedUserAndExistentGoalId_WhenUpdatingStatusToReadyGoal_ThenReturnsNoContent_And_UpdatesTheGoalStastusToReady()
    {
        Goal aliceGoal = await Given.AGoalInTheDatabase(UserBuilder.AliceUserId);

        HttpClient httpClient = Given.CreateClient()
                                     .WithIdentity(UserBuilder.AliceClaims);


        var (response, _) = await httpClient.PUTAsync<ReadyGoalEndpoint, Guid, EmptyResponse>(aliceGoal.Id);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        GoalsDbContext context = Given.GetGoalsDbContext();
        Goal updatedGoal = context.Goals.First(x => x.Id == aliceGoal.Id);

        updatedGoal.GoalStatus.Should().Be(GoalStatus.Ready);
        updatedGoal.Should().BeEquivalentTo(aliceGoal, options => options.Excluding(x => x.GoalStatus));
    }
}
