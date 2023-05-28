using FastEndpoints;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using Vita.Goals.Api.Endpoints.Goals.Complete;
using Vita.Goals.Api.Endpoints.Goals.GetById;
using Vita.Goals.Application.Queries.Goals;
using Vita.Goals.Domain.Aggregates.Goals;
using Vita.Goals.FunctionalTests.Fixtures.Builders;
using Vita.Goals.FunctionalTests.Fixtures.Extensions;
using Vita.Goals.FunctionalTests.Goals.Fixtures;

namespace Vita.Goals.FunctionalTests.Goals;

[Collection(nameof(GoalsTestCollection))]
public class GetGoalTests
{
    private GoalsTestsFixture Given { get; }

    public GetGoalTests(GoalsTestsFixture goalsTestsFixture)
    {
        Given = goalsTestsFixture;
    }

    [Fact]
    public async Task GivenUnauthenticatedUser_WhenGettingGoal_ThenReturnsUnauthorized()
    {
        Goal goal = await Given.AGoalInTheDatabase(UserBuilder.AliceUserId);

        HttpClient httpClient = Given.CreateClient();

        string endpointUri = IEndpoint.TestURLFor<GetGoalEndpoint>()
                                      .Replace("{id}", goal.Id.ToString());

        var (response, _) = await httpClient.GETAsync<EmptyRequest, GoalDto>(endpointUri, default);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GivenUnauthorizedUser_WhenGettingGoal_ThenReturnsForbidden()
    {
        Goal goal = await Given.AGoalInTheDatabase(UserBuilder.AliceUserId);

        HttpClient httpClient = Given.CreateClient()
                                     .WithIdentity(UserBuilder.UnauthorizedUserClaims);

        string endpointUri = IEndpoint.TestURLFor<GetGoalEndpoint>()
                                      .Replace("{id}", goal.Id.ToString());

        var (response, _) = await httpClient.GETAsync<EmptyRequest, GoalDto>(endpointUri, default);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GivenGoalOfOtherUser_WhenGettingGoal_ThenReturnsForbidden()
    {
        await Given.CleanDatabase();
        Goal aliceGoal = await Given.AGoalInTheDatabase(UserBuilder.AliceUserId);

        HttpClient httpClient = Given.CreateClient()
                                     .WithIdentity(UserBuilder.BobClaims);

        string endpointUri = IEndpoint.TestURLFor<GetGoalEndpoint>()
                                      .Replace("{id}", aliceGoal.Id.ToString());

        var (response, _) = await httpClient.GETAsync<EmptyRequest, GoalDto>(endpointUri, default);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GivenAuthorizedUserButUnexistentGoalWithId_WhenGettingGoal_ThenReturnsNotFound()
    {
        await Given.CleanDatabase();

        HttpClient httpClient = Given.CreateClient()
                                     .WithIdentity(UserBuilder.BobClaims);

        string endpointUri = IEndpoint.TestURLFor<GetGoalEndpoint>()
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
    public async Task GivenAUserGoal_WhenGettingGoal_ThenReturnsOkWithTheGoal()
    {
        await Given.CleanDatabase();
        Goal goal = await Given.AGoalWithDateTimeIntervalInTheDatabase(UserBuilder.AliceUserId);

        HttpClient httpClient = Given.CreateClient()
                                     .WithIdentity(UserBuilder.AliceClaims);


        string endpointUri = IEndpoint.TestURLFor<GetGoalEndpoint>()
                                      .Replace("{id}", goal.Id.ToString());

        var (response, goalDto) = await httpClient.GETAsync<EmptyRequest, GoalDto>(endpointUri, default);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        goalDto.Should().NotBeNull();
        goalDto.Id.Should().Be(goal.Id);
        goalDto.Title.Should().Be(goal.Title);
        goalDto.Description.Should().Be(goal.Description);
        goalDto.AimDateStart.Should().Be(goal.AimDate!.Start);
        goalDto.AimDateEnd.Should().Be(goal.AimDate!.End);
        goalDto.Status.Should().Be(GoalStatus.ToBeDefined.Name);
        goalDto.CreatedOn.Should().BeCloseTo(goal.CreatedOn, TimeSpan.FromSeconds(1));
    }
   
}
