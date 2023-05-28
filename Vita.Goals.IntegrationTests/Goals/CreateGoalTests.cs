using FastEndpoints;
using FluentAssertions;
using System.Net;
using Vita.Goals.Api.Endpoints.Goals.Create;
using Vita.Goals.Domain.Aggregates.Goals;
using Vita.Goals.FunctionalTests.Fixtures.Builders;
using Vita.Goals.FunctionalTests.Fixtures.Extensions;
using Vita.Goals.FunctionalTests.Goals.Fixtures;
using Vita.Goals.Infrastructure.Sql;

namespace Vita.Goals.FunctionalTests.Goals;

[Collection(nameof(GoalsTestCollection))]
public class CreateGoalTests
{
    private GoalsTestsFixture Given { get; }

    public CreateGoalTests(GoalsTestsFixture goalsTestsFixture)
    {
        Given = goalsTestsFixture;
    }

    [Fact]
    public async Task GivenUnauthenticatedUser_WhenCreatingGoal_ThenReturnsUnauthorized()
    {
        CreateGoalRequest request = GoalsTestsFixture.BuildCreateGoalRequest();

        HttpClient httpClient = Given.CreateClient();

        var (response, _) = await httpClient.POSTAsync<CreateGoalEndpoint, CreateGoalRequest, EmptyResponse>(request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GivenUnauthorizedUser_WhenCreatingGoal_ThenReturnsForbidden()
    {
        CreateGoalRequest request = GoalsTestsFixture.BuildCreateGoalRequest();

        HttpClient httpClient = Given.CreateClient()
                                     .WithIdentity(UserBuilder.UnauthorizedUserClaims);

        var (response, _) = await httpClient.POSTAsync<CreateGoalEndpoint, CreateGoalRequest, EmptyResponse>(request);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GivenAuthorizedUser_WhenCreatingGoal_ThenCreatesTheGoal()
    {
        await Given.CleanDatabase();
        CreateGoalRequest request = GoalsTestsFixture.BuildCreateGoalRequest();

        HttpClient httpClient = Given.CreateClient()
                                     .WithIdentity(UserBuilder.AliceClaims);

        var (response, _) = await httpClient.POSTAsync<CreateGoalEndpoint, CreateGoalRequest, EmptyResponse>(request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        GoalsDbContext context = Given.GetGoalsDbContext();
        Goal goal = context.Goals.Single();

        goal.GoalStatus.Should().Be(GoalStatus.ToBeDefined);
        goal.Title.Should().Be(request.Title);
        goal.Description.Should().Be(request.Description);
        goal.AimDate.Start.Should().Be(request.AimDateStart);
        goal.AimDate.End.Should().Be(request.AimDateEnd);
        goal.CreatedBy.Should().Be(UserBuilder.AliceUserId);

        response.Headers.Location.Should().Be($"/api/goals/{goal.Id}");
    }
}
