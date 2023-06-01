using FastEndpoints;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using Vita.Goals.Api.Endpoints.Goals.Delete;
using Vita.Goals.Domain.Aggregates.Goals;
using Vita.Goals.FunctionalTests.Fixtures.Builders;
using Vita.Goals.FunctionalTests.Fixtures.Extensions;
using Vita.Goals.FunctionalTests.Goals.Fixtures;

namespace Vita.Goals.FunctionalTests.Goals;

[Collection(nameof(GoalsTestCollection))]
public class DeleteGoalTests
{
    private GoalsTestsFixture Given { get; }

    public DeleteGoalTests(GoalsTestsFixture goalsTestsFixture)
    {
        Given = goalsTestsFixture;
    }

    [Fact]
    public async Task GivenUnauthenticatedUser_WhenDeletingGoal_ThenReturnsUnauthorized()
    {
        Goal goal = await Given.AGoalInTheDatabase(UserBuilder.AliceUserId);

        HttpClient httpClient = Given.CreateClient();

        string endpointUri = IEndpoint.TestURLFor<DeleteGoalEndpoint>()
                                      .Replace("{id}", goal.Id.ToString());

        var (response, _) = await httpClient.DELETEAsync<EmptyRequest, EmptyResponse>(endpointUri, default);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GivenUnauthorizedUser_WhenDeletingGoal_ThenReturnsForbidden()
    {
        Goal goal = await Given.AGoalInTheDatabase(UserBuilder.AliceUserId);

        HttpClient httpClient = Given.CreateClient()
                                     .WithIdentity(UserBuilder.UnauthorizedUserClaims);

        string endpointUri = IEndpoint.TestURLFor<DeleteGoalEndpoint>()
                              .Replace("{id}", goal.Id.ToString());

        var (response, _) = await httpClient.DELETEAsync<EmptyRequest, EmptyResponse>(endpointUri, default);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }


    [Fact]
    public async Task GivenAUserGoal_WhenDeletingGoal_ThenReturnsNoContent()
    {
        Goal goal = await Given.AGoalInTheDatabase(UserBuilder.AliceUserId);

        HttpClient httpClient = Given.CreateClient()
                                     .WithIdentity(UserBuilder.AliceClaims);

        string endpointUri = IEndpoint.TestURLFor<DeleteGoalEndpoint>()
                                      .Replace("{id}", goal.Id.ToString());

        var (response, _) = await httpClient.DELETEAsync<EmptyRequest, EmptyResponse>(endpointUri, default);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task GivenANoExistentUserGoal_WhenDeletingGoal_ThenReturnsNotFound()
    {
        HttpClient httpClient = Given.CreateClient()
                                     .WithIdentity(UserBuilder.AliceClaims);

        string endpointUri = IEndpoint.TestURLFor<DeleteGoalEndpoint>()
                              .Replace("{id}", Guid.NewGuid().ToString());

        var (response, _) = await httpClient.DELETEAsync<EmptyRequest, EmptyResponse>(endpointUri, default);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        Microsoft.AspNetCore.Mvc.ProblemDetails problem = (await response.Content.ReadFromJsonAsync<Microsoft.AspNetCore.Mvc.ProblemDetails>())!;

        problem.Status.Should().Be((int)HttpStatusCode.NotFound);
        problem.Title.Should().Be("The given key was not present in the dictionary.");
        problem.Detail.Should().NotBeNullOrEmpty();
        problem.Instance.Should().NotBeNullOrEmpty();
    }
}
