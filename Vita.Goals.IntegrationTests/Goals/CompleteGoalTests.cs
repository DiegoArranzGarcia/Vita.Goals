using FastEndpoints;
using FluentAssertions;
using System.Net;
using Vita.Goals.Api.Endpoints.Goals.Complete;
using Vita.Goals.Api.Endpoints.Goals.Create;
using Vita.Goals.Domain.Aggregates.Goals;
using Vita.Goals.FunctionalTests.Fixtures.Builders;
using Vita.Goals.FunctionalTests.Fixtures.Extensions;
using Vita.Goals.Infrastructure.Sql;

namespace Vita.Goals.FunctionalTests.Goals;

public class CompleteGoalTests : IClassFixture<GoalsTestsFixture>
{
    private GoalsTestsFixture Given { get; }

    public CompleteGoalTests(GoalsTestsFixture goalsTestsFixture)
    {
        Given = goalsTestsFixture;
    }

    [Fact]
    public async Task GivenUnauthenticatedUser_WhenCompletingGoal_ThenReturnsUnauthorized()
    {
        Goal alreadyExistentGoal = await Given.AGoalInTheDatabase(UserBuilder.AliceUserId);
        CompleteGoalRequest request = new() { GoalId = alreadyExistentGoal.Id };

        HttpClient httpClient = Given.CreateClient();

        var (reponse, _) = await httpClient.PUTAsync<CompleteGoalEndpoint, CompleteGoalRequest, EmptyResponse>(request);

        reponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GivenUnauthorizedUser_WhenCompletingGoal_ThenReturnsForbidden()
    {
        Goal alreadyExistentGoal = await Given.AGoalInTheDatabase(UserBuilder.AliceUserId);
        CompleteGoalRequest request = new() { GoalId = alreadyExistentGoal.Id };

        HttpClient httpClient = Given.CreateClient()
                                     .WithIdentity(UserBuilder.UnauthorizedUserClaims);

        var (reponse, _) = await httpClient.PUTAsync<CompleteGoalEndpoint, CompleteGoalRequest, EmptyResponse>(request);

        reponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }


    [Fact]
    public async Task GivenGoalOfOtherUser_WhenCompletingGoal_ThenReturnsForbidden()
    {
        Goal alreadyExistentGoal = await Given.AGoalInTheDatabase(UserBuilder.AliceUserId);
        CompleteGoalRequest request = new() { GoalId = alreadyExistentGoal.Id };

        HttpClient httpClient = Given.CreateClient()
                                     .WithIdentity(UserBuilder.BobClaims);

        var (reponse, _) = await httpClient.PUTAsync<CompleteGoalEndpoint, CompleteGoalRequest, EmptyResponse>(request);

        reponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GivenAuthorizedUserButUnexistentGoalWithId_WhenCompletingGoal_ThenReturnsNotFound()
    {
        Goal alreadyExistentGoal = await Given.AGoalInTheDatabase(UserBuilder.AliceUserId);
        CompleteGoalRequest request = new() { GoalId = Guid.NewGuid() };

        HttpClient httpClient = Given.CreateClient()
                                     .WithIdentity(UserBuilder.BobClaims);

        var (reponse, _) = await httpClient.PUTAsync<CompleteGoalEndpoint, CompleteGoalRequest, EmptyResponse>(request);

        reponse.StatusCode.Should().Be(HttpStatusCode.NotFound);  
    }

    [Fact]
    public async Task GivenAuthorizedUserAndExistentGoalId_WhenCompletingGoal_ThenReturnsNoContent_And_UpdatesTheGoalStastusToComplete()
    {
        Goal alreadyExistentGoal = await Given.AGoalInTheDatabase(UserBuilder.AliceUserId);
        CompleteGoalRequest request = new() { GoalId = alreadyExistentGoal.Id };

        HttpClient httpClient = Given.CreateClient()
                                     .WithIdentity(UserBuilder.AliceClaims);


        var (reponse, _) = await httpClient.PUTAsync<CompleteGoalEndpoint, CompleteGoalRequest, EmptyResponse>(request);

        reponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        GoalsDbContext context = Given.GetGoalsDbContext();
        Goal updatedGoal = context.Goals.First(x => x.Id == alreadyExistentGoal.Id);

        updatedGoal.GoalStatus.Should().Be(GoalStatus.Completed);
        updatedGoal.Should().BeEquivalentTo(alreadyExistentGoal, options => options.Excluding(x => x.GoalStatus));
    }
}
