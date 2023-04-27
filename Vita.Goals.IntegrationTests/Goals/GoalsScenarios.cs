using FluentAssertions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Net;
using Vita.Goals.Application.Queries.Goals;
using Vita.Goals.Domain.Aggregates.Goals;
using Vita.Goals.FunctionalTests.Authentication;
using Vita.Goals.FunctionalTests.Builders;

namespace Vita.Goals.FunctionalTests.Goals;

public class GoalsScenarios : IClassFixture<GoalsTestsFixture>
{
    private readonly GoalsTestsFixture _given;

    public GoalsScenarios(GoalsTestsFixture goalsTestsFixture)
    {
        _given = goalsTestsFixture;
    }

    [Fact]
    public async Task GivenUnauthenticatedUser_WhenGettingAllGoals_ThenReturnsUnauthorized()
    {
        HttpResponseMessage response = await _given.Server.CreateRequest(Get.GetGoals)
                                                          .GetAsync();

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GivenUnauthorizedUser_WhenGettingAllGoals_ThenReturnsForbidden()
    {
        HttpResponseMessage response = await _given.Server.CreateRequest(Get.GetGoals)
                                                          .AddHeader("Authorization", $"{JwtBearerDefaults.AuthenticationScheme} {UsersTokens.UnauthorizedToken}")
                                                          .GetAsync();

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GivenAuthorizedUser_WhenGettingAllGoals_ThenReturnsOkWithAllGoals()
    {
        await _given.CleanDatabase();

        IReadOnlyCollection<Goal> goals = await _given.GoalsInTheDatabase(UserBuilder.AliceUserId);

        HttpResponseMessage response = await _given.Server.CreateRequest(Get.GetGoals)
                                                          .AddHeader("Authorization", $"{JwtBearerDefaults.AuthenticationScheme} {UsersTokens.AliceToken}")
                                                          .GetAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        IEnumerable<GoalDto> goalsRetrieved = await response.ReadContentAsAsync<IEnumerable<GoalDto>>();
        goalsRetrieved.Should().NotBeEmpty();
        goalsRetrieved.Should().HaveSameCount(goals);
    }

    [Fact]
    public async Task GivenAuthorizedUserAndGoalsForOtherUser_WhenGettingAllGoals_ThenReturnsOkWithNoGoals()
    {
        await _given.CleanDatabase();

        _ = _given.GoalsInTheDatabase(UserBuilder.AliceUserId);

        HttpResponseMessage response = await _given.Server.CreateRequest(Get.GetGoals)
                                                          .AddHeader("Authorization", $"{JwtBearerDefaults.AuthenticationScheme} {UsersTokens.BobToken}")
                                                          .GetAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        IEnumerable<GoalDto> goalsRetrieved = await response.ReadContentAsAsync<IEnumerable<GoalDto>>();
        goalsRetrieved.Should().BeEmpty();
    }

    static class Get
    {
        public static string GetGoals = "api/Goals";
    }
}
