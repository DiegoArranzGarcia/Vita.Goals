﻿using FastEndpoints;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using Vita.Goals.Api.Endpoints.Goals.Complete;
using Vita.Goals.Api.Endpoints.Goals.Delete;
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

        string endpointUri = IEndpoint.TestURLFor<GetGoalTasksEndpoint>()
                              .Replace("{id}", goal.Id.ToString());

        var (response, _) = await httpClient.GETAsync<EmptyRequest, IEnumerable<GoalTaskDto>>(endpointUri, default);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GivenUnauthorizedUser_WhenGettingGoalTasks_ThenReturnsForbidden()
    {
        Goal goal = await Given.AGoalInTheDatabase(UserBuilder.AliceUserId);

        HttpClient httpClient = Given.CreateClient()
                                     .WithIdentity(UserBuilder.UnauthorizedUserClaims);

        string endpointUri = IEndpoint.TestURLFor<GetGoalTasksEndpoint>()
                                      .Replace("{id}", goal.Id.ToString());

        var (response, _) = await httpClient.GETAsync<EmptyRequest, IEnumerable<GoalTaskDto>>(endpointUri, default);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GivenGoalOfOtherUser_WhenGettingGoalTasks_ThenReturnsForbidden()
    {
        await Given.CleanDatabase();
        Goal aliceGoal = await Given.AGoalInTheDatabase(UserBuilder.AliceUserId);

        HttpClient httpClient = Given.CreateClient()
                                     .WithIdentity(UserBuilder.BobClaims);

        string endpointUri = IEndpoint.TestURLFor<GetGoalTasksEndpoint>()
                                      .Replace("{id}", aliceGoal.Id.ToString());

        var (response, _) = await httpClient.GETAsync<EmptyRequest, IEnumerable<GoalTaskDto>>(endpointUri, default);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GivenAuthorizedUserButUnexistentGoalWithId_WhenCompletingGoal_ThenReturnsNotFound()
    {
        await Given.CleanDatabase();

        HttpClient httpClient = Given.CreateClient()
                                     .WithIdentity(UserBuilder.BobClaims);

        string endpointUri = IEndpoint.TestURLFor<GetGoalTasksEndpoint>()
                              .Replace("{id}", Guid.NewGuid().ToString());

        var (response, problem) = await httpClient.GETAsync<EmptyRequest, Microsoft.AspNetCore.Mvc.ProblemDetails>(endpointUri, default);

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


        string endpointUri = IEndpoint.TestURLFor<GetGoalTasksEndpoint>()
                                      .Replace("{id}", aliceGoal.Id.ToString());

        var (response, goalTasksDto) = await httpClient.GETAsync<EmptyRequest, IEnumerable<GoalTaskDto>>(endpointUri, default);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        goalTasksDto.Should().NotBeNullOrEmpty()
                    .And.HaveSameCount(tasks);
    }
}
