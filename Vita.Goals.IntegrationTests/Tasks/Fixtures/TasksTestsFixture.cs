using Bogus;
using Microsoft.Extensions.DependencyInjection;
using Vita.Common;
using Vita.Goals.Api.Endpoints.Tasks.Create;
using Vita.Goals.Api.Endpoints.Tasks.Update;
using Vita.Goals.Domain.Aggregates.Goals;
using Vita.Goals.Domain.ValueObjects;
using Vita.Goals.FunctionalTests.Fixtures.Builders;
using Vita.Goals.FunctionalTests.Fixtures.WebApplicationFactories;
using Vita.Goals.Infrastructure.Sql;

namespace Vita.Goals.FunctionalTests.Tasks.Fixtures;

public class TasksTestsFixture : WebApiApplicationFactory
{
    public async Task<Goal> AGoalInTheDatabase(Guid userId)
    {
        using IServiceScope scope = Services.CreateScope();
        using GoalsDbContext context = scope.ServiceProvider.GetRequiredService<GoalsDbContext>();

        Goal goal = new GoalBuilder().WithUserId(userId).Build();

        context.Goals.Add(goal);
        await context.SaveChangesAsync();

        return goal;
    }

    public async Task<Domain.Aggregates.Tasks.Task> ATaskInTheDatabase(Guid userId)
    {
        using IServiceScope scope = Services.CreateScope();
        using GoalsDbContext context = scope.ServiceProvider.GetRequiredService<GoalsDbContext>();

        IReadOnlyCollection<Domain.Aggregates.Tasks.Task> tasks = await TasksInTheDatabase(userId, count: 1);

        return tasks.First();
    }

    public async Task<IReadOnlyCollection<Domain.Aggregates.Tasks.Task>> TasksInTheDatabase(Guid userId, int count = 15)
    {
        using IServiceScope scope = Services.CreateScope();
        using GoalsDbContext context = scope.ServiceProvider.GetRequiredService<GoalsDbContext>();

        Goal goal = new GoalBuilder().WithUserId(userId).Build();

        List<Domain.Aggregates.Tasks.Task> tasks = Enumerable.Range(0, count)
                                                             .Select(_ => new TaskBuilder().WithGoal(goal).Build())
                                                             .ToList();

        context.Tasks.AddRange(tasks);
        await context.SaveChangesAsync();

        return tasks;
    }

    public async Task<Domain.Aggregates.Tasks.Task> ATaskWithPlannedDateInTheDatabase(Guid userId)
    {
        using IServiceScope scope = Services.CreateScope();
        using GoalsDbContext context = scope.ServiceProvider.GetRequiredService<GoalsDbContext>();

        DateTimeInterval range = new DateTimeIntervalBuilder().Build();
        Goal goal = new GoalBuilder().WithUserId(userId).Build();

        Domain.Aggregates.Tasks.Task task = new TaskBuilder().WithGoal(goal)
                                                             .WithPlannedDate(range)
                                                             .Build();

        context.Tasks.Add(task);
        await context.SaveChangesAsync();

        return task;
    }

    public static CreateTaskRequest BuildCreateTaskRequest(Guid goalId) => new Faker<CreateTaskRequest>()
        .CustomInstantiator(faker =>
        {
            DateTimeInterval range = new DateTimeIntervalBuilder().Build();

            return new CreateTaskRequest
            (
                Title: faker.Lorem.Sentence(5),
                GoalId: goalId,
                PlannedDateStart: range.Start,
                PlannedDateEnd: range.End
            );
        });

    public static UpdateTaskRequest BuildCreateUpdateRequest(Guid goalId) => new Faker<UpdateTaskRequest>()
        .CustomInstantiator(faker =>
        {
            DateTimeInterval range = new DateTimeIntervalBuilder().Build();

            return new UpdateTaskRequest
            (
                Title: faker.Lorem.Sentence(5),
                GoalId: goalId,
                PlannedDateStart: range.Start,
                PlannedDateEnd: range.End
            );
        });
}