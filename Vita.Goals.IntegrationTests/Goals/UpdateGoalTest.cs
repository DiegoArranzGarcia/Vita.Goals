using FastEndpoints;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using Vita.Goals.Api.Endpoints.Goals.Update;
using Vita.Goals.Domain.Aggregates.Goals;
using Vita.Goals.FunctionalTests.Extensions;
using Vita.Goals.FunctionalTests.Fixtures.Builders;
using Vita.Goals.FunctionalTests.Fixtures.Extensions;
using Vita.Goals.FunctionalTests.Goals.Fixtures;
using Vita.Goals.Infrastructure.Sql;

namespace Vita.Goals.FunctionalTests.Goals;


[Collection(nameof(GoalsTestCollection))]
public class UpdateGoalTests
{
    private GoalsTestsFixture Given { get; }

    public UpdateGoalTests(GoalsTestsFixture goalsTestsFixture)
    {
        Given = goalsTestsFixture;
    }

    [Fact]
    public async Task GivenUnauthenticatedUser_WhenToUpdatingGoal_ThenReturnsUnauthorized()
    {
        Goal aliceGoal = await Given.AGoalInTheDatabase(UserBuilder.AliceUserId);
        UpdateGoalRequest request = GoalsTestsFixture.BuildCreateUpdateRequest();

        HttpClient httpClient = Given.CreateClient();

        string endpointUri = IEndpoint.TestURLFor<UpdateGoalEndpoint>()
                                      .Replace("{id}", aliceGoal.Id.ToString());

        var (response, _) = await httpClient.PATCHAsync<UpdateGoalRequest, EmptyResponse>(requestUri: endpointUri, request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GivenUnauthorizedUser_WhenUpdatingGoal_ThenReturnsForbidden()
    {
        Goal aliceGoal = await Given.AGoalInTheDatabase(UserBuilder.AliceUserId);
        UpdateGoalRequest request = GoalsTestsFixture.BuildCreateUpdateRequest();

        HttpClient httpClient = Given.CreateClient()
                                     .WithIdentity(UserBuilder.UnauthorizedUserClaims);

        string endpointUri = IEndpoint.TestURLFor<UpdateGoalEndpoint>()
                                      .Replace("{id}", aliceGoal.Id.ToString());

        var (response, _) = await httpClient.PATCHAsync<UpdateGoalRequest, EmptyResponse>(requestUri: endpointUri, request);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }


    [Fact]
    public async Task GivenGoalOfOtherUser_WhenUpdatingGoal_ThenReturnsForbidden()
    {
        Goal aliceGoal = await Given.AGoalInTheDatabase(UserBuilder.AliceUserId);
        UpdateGoalRequest request = GoalsTestsFixture.BuildCreateUpdateRequest();

        HttpClient httpClient = Given.CreateClient()
                                     .WithIdentity(UserBuilder.BobClaims);

        string endpointUri = IEndpoint.TestURLFor<UpdateGoalEndpoint>()
                                      .Replace("{id}", aliceGoal.Id.ToString());

        var (response, _) = await httpClient.PATCHAsync<UpdateGoalRequest, EmptyResponse>(requestUri: endpointUri, request);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GivenAuthorizedUserButUnexistentGoalWithId_WhenUpdatingGoal_ThenReturnsNotFound()
    {
        UpdateGoalRequest request = GoalsTestsFixture.BuildCreateUpdateRequest();

        HttpClient httpClient = Given.CreateClient()
                                     .WithIdentity(UserBuilder.BobClaims);

        string endpointUri = IEndpoint.TestURLFor<UpdateGoalEndpoint>()
                                      .Replace("{id}", Guid.NewGuid().ToString());

        var (response, _) = await httpClient.PATCHAsync<UpdateGoalRequest, EmptyResponse>(requestUri: endpointUri, request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        Microsoft.AspNetCore.Mvc.ProblemDetails problem = (await response.Content.ReadFromJsonAsync<Microsoft.AspNetCore.Mvc.ProblemDetails>())!;

        problem.Should().NotBeNull();
        problem.Status.Should().Be((int)HttpStatusCode.NotFound);
        problem.Title.Should().Be("The given key was not present in the dictionary.");
        problem.Detail.Should().NotBeNullOrEmpty();
        problem.Instance.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GivenAuthorizedUserAndExistentGoalId_WhenUpdatingGoal_ThenReturnsNoContent_And_UpdatesTheGoalStastusToUpdate()
    {
        Goal aliceGoal = await Given.AGoalInTheDatabase(UserBuilder.AliceUserId);
        UpdateGoalRequest request = GoalsTestsFixture.BuildCreateUpdateRequest();

        HttpClient httpClient = Given.CreateClient()
                                     .WithIdentity(UserBuilder.AliceClaims);

        string endpointUri = IEndpoint.TestURLFor<UpdateGoalEndpoint>()
                                      .Replace("{id}", aliceGoal.Id.ToString());

        var (response, _) = await httpClient.PATCHAsync<UpdateGoalRequest, EmptyResponse>(requestUri: endpointUri, request);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        GoalsDbContext context = Given.GetGoalsDbContext();
        Goal updatedGoal = context.Goals.First(x => x.Id == aliceGoal.Id);

        updatedGoal.Title.Should().Be(request.Title);
        updatedGoal.Description.Should().Be(request.Description);
        updatedGoal.AimDate.Should().NotBeNull();
        updatedGoal.AimDate!.Start.Should().Be(request.AimDateStart);
        updatedGoal.AimDate!.End.Should().Be(request.AimDateEnd);
        updatedGoal.GoalStatus.Should().Be(aliceGoal.GoalStatus);
        updatedGoal.CreatedBy.Should().Be(aliceGoal.CreatedBy);
        updatedGoal.CreatedOn.Should().BeCloseTo(aliceGoal.CreatedOn, TimeSpan.FromSeconds(1));
    }
}