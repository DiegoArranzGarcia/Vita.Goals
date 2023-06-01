using Bogus;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;
using Vita.Common;
using Vita.Goals.Api.Endpoints.Goals.Create;
using Vita.Goals.Api.Endpoints.Goals.Update;
using Vita.Goals.Domain.Aggregates.Goals;
using Vita.Goals.Domain.ValueObjects;
using Vita.Goals.FunctionalTests.Fixtures.Builders;
using Vita.Goals.FunctionalTests.Fixtures.WebApplicationFactories;
using Vita.Goals.Infrastructure.Sql;

namespace Vita.Goals.FunctionalTests.Goals.Fixtures;

public class GoalsTestsFixture : WebApiApplicationFactory
{
    public async Task<Goal> AGoalInTheDatabase(Guid userId)
    {
        using IServiceScope scope = Services.CreateScope();
        using GoalsDbContext context = scope.ServiceProvider.GetRequiredService<GoalsDbContext>();

        IReadOnlyCollection<Goal> goals = await GoalsInTheDatabase(userId, count: 1);

        return goals.First();
    }

    public async Task<Goal> AGoalWithDateTimeIntervalInTheDatabase(Guid userId)
    {
        using IServiceScope scope = Services.CreateScope();
        using GoalsDbContext context = scope.ServiceProvider.GetRequiredService<GoalsDbContext>();

        DateTimeInterval range = new DateTimeIntervalBuilder().Build();

        Goal goal = new GoalBuilder().WithUserId(userId)
                                     .WithRange(range)
                                     .Build();

        EntityEntry<Goal> entry = context.Goals.Add(goal);
        await context.SaveChangesAsync();

        return entry.Entity;
    }

    public async Task<IReadOnlyCollection<Goal>> GoalsInTheDatabase(Guid userId, int count = 15)
    {
        using IServiceScope scope = Services.CreateScope();
        using GoalsDbContext context = scope.ServiceProvider.GetRequiredService<GoalsDbContext>();

        List<Goal> goals = Enumerable.Range(0, count)
                                     .Select(_ => new GoalBuilder().WithUserId(userId).Build())
                                     .ToList();

        context.Goals.AddRange(goals);
        await context.SaveChangesAsync();

        return goals;
    }

    public static CreateGoalRequest BuildCreateGoalRequest() => new Faker<CreateGoalRequest>()
        .CustomInstantiator(faker =>
        {
            DateTimeInterval range = new DateTimeIntervalBuilder().Build();

            return new CreateGoalRequest
            (
                Title: faker.Lorem.Sentence(5),
                Description: faker.Lorem.Sentence(15),
                AimDateStart: range.Start,
                AimDateEnd: range.End
            );
        });

    public static UpdateGoalRequest BuildCreateUpdateRequest() => new Faker<UpdateGoalRequest>()
        .CustomInstantiator(faker =>
        {
            DateTimeInterval range = new DateTimeIntervalBuilder().Build();

            return new UpdateGoalRequest
            (
                Title: faker.Lorem.Sentence(5),
                Description: faker.Lorem.Sentence(15),
                AimDateStart: range.Start,
                AimDateEnd: range.End
            );
        });

    public async Task<IReadOnlyCollection<Domain.Aggregates.Tasks.Task>> GoalTasksForGoalInTheDatabase(Guid goalId, int count = 15)
    {
        using IServiceScope scope = Services.CreateScope();
        using GoalsDbContext context = scope.ServiceProvider.GetRequiredService<GoalsDbContext>();

        Goal? goal = await context.Goals.FindAsync(goalId);

        List<Domain.Aggregates.Tasks.Task> tasks = new Faker<Domain.Aggregates.Tasks.Task>().CustomInstantiator(faker => new Domain.Aggregates.Tasks.Task
        (
            title: faker.Lorem.Sentence(5),
            associatedTo: goal!
        )).Generate(count);

        context.Tasks.AddRange(tasks);
        await context.SaveChangesAsync();

        return tasks;
    }
}