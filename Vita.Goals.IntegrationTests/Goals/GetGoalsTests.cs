using FastEndpoints;
using FluentAssertions;
using System.Net;
using Vita.Goals.Api.Endpoints.Goals.Get;
using Vita.Goals.Application.Queries.Goals;
using Vita.Goals.Domain.Aggregates.Goals;
using Vita.Goals.FunctionalTests.Fixtures.Builders;
using Vita.Goals.FunctionalTests.Fixtures.Extensions;
using Vita.Goals.FunctionalTests.Goals.Fixtures;

namespace Vita.Goals.FunctionalTests.Goals;

[Collection(nameof(GoalsTestCollection))]
public class GetTasksTests
{
    private GoalsTestsFixture Given { get; }

    public GetTasksTests(GoalsTestsFixture goalsTestsFixture)
    {
        Given = goalsTestsFixture;
    }

    [Fact]
    public async Task GivenUnauthenticatedUser_WhenGettingAllGoals_ThenReturnsUnauthorized()
    {
        HttpClient httpClient = Given.CreateClient();

        (HttpResponseMessage response, _) =
          await httpClient.GETAsync<GetGoalsEndpoint, GetGoalsRequest, IEnumerable<GoalDto>>(new GetGoalsRequest());

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GivenUnauthorizedUser_WhenGettingAllGoals_ThenReturnsForbidden()
    {
        HttpClient httpClient = Given.CreateClient().WithIdentity(UserBuilder.UnauthorizedUserClaims);

        (HttpResponseMessage response, _) =
          await httpClient.GETAsync<GetGoalsEndpoint, GetGoalsRequest, IEnumerable<GoalDto>>(new GetGoalsRequest());

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GivenAuthorizedUser_WhenGettingAllGoals_ThenReturnsOkWithAllGoals()
    {
        await Given.CleanDatabase();

        IReadOnlyCollection<Goal> databaseGoals = await Given.GoalsInTheDatabase(UserBuilder.AliceUserId);

        HttpClient httpClient = Given.CreateClient().WithIdentity(UserBuilder.AliceClaims);

        (HttpResponseMessage response, IEnumerable<GoalDto>? goals) =
            await httpClient.GETAsync<GetGoalsEndpoint, GetGoalsRequest, IEnumerable<GoalDto>>(new GetGoalsRequest());

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        goals.Should().NotBeNull()
                  .And.NotBeEmpty()
                  .And.HaveSameCount(databaseGoals);
    }

    [Theory]
    [InlineData(0, 0, true)]
    [InlineData(1, 0, false)]
    [InlineData(0, 1, false)]
    public async Task GivenAGoalWithStartEndDates_WhenGettingGoalsWithoutShowCompletedFilter_ThenDoesntReturnTheGoal(
        int endDatePlusDays,
        int startDateLessDays,
        bool shouldBeRetreived)
    {
        await Given.CleanDatabase();

        Goal goal = await Given.AGoalWithDateTimeIntervalInTheDatabase(UserBuilder.AliceUserId);

        HttpClient httpClient = Given.CreateClient().WithIdentity(UserBuilder.AliceClaims);
        GetGoalsRequest request = new
        (
            StartDate: goal.AimDate.End.AddDays(endDatePlusDays),
            EndDate: goal.AimDate.Start.AddDays(-startDateLessDays)
        );

        (HttpResponseMessage response, IEnumerable<GoalDto>? goals) =
            await httpClient.GETAsync<GetGoalsEndpoint, GetGoalsRequest, IEnumerable<GoalDto>>(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        goals.Should().NotBeNull();
        goals.Should().HaveCount(shouldBeRetreived ? 1 : 0);
    }

    [Fact]
    public async Task GivenAuthorizedUserAndGoalsForOtherUser_WhenGettingAllGoals_ThenReturnsOkWithNoGoals()
    {
        await Given.CleanDatabase();
        _ = Given.GoalsInTheDatabase(UserBuilder.AliceUserId);

        HttpClient httpClient = Given.CreateClient().WithIdentity(UserBuilder.BobClaims);

        (HttpResponseMessage response, IEnumerable<GoalDto>? goals) =
            await httpClient.GETAsync<GetGoalsEndpoint, GetGoalsRequest, IEnumerable<GoalDto>>(new GetGoalsRequest());

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        goals.Should().NotBeNull().And.BeEmpty();
    }
}
