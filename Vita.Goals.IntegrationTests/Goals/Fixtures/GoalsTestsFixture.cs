using Bogus;
using Bogus.DataSets;
using Vita.Goals.Api.Endpoints.Goals.Create;
using Vita.Goals.Domain.Aggregates.Goals;
using Vita.Goals.Domain.ValueObjects;
using Vita.Goals.FunctionalTests.Fixtures.WebApplicationFactories;
using Vita.Goals.Infrastructure.Sql;

namespace Vita.Goals.FunctionalTests.Goals;

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

        DateTimeInterval range = CreateDateTimeInterval();

        Goal goal = new Faker<Goal>().CustomInstantiator(faker => new Goal
            (
                title: faker.Lorem.Sentence(5),
                createdBy: userId,
                description: faker.Lorem.Sentence(15),
                aimDate: range
            ))
            .Generate();

        context.Goals.Add(goal);
        await context.SaveChangesAsync();

        return goal;
    }

    public async Task<IReadOnlyCollection<Goal>> GoalsInTheDatabase(Guid userId, int count = 15)
    {
        using IServiceScope scope = Services.CreateScope();
        using GoalsDbContext context = scope.ServiceProvider.GetRequiredService<GoalsDbContext>();

        List<Goal> goals = new Faker<Goal>().CustomInstantiator(faker => new Goal(title: faker.Lorem.Sentence(5), createdBy: userId, description: faker.Lorem.Sentence(15)))
                                            .Generate(count);

        context.Goals.AddRange(goals);
        await context.SaveChangesAsync();

        return goals;
    }

    public static CreateGoalRequest BuildCreateGoalRequest() => new Faker<CreateGoalRequest>()
        .CustomInstantiator(faker =>
        {
            DateTimeInterval range = CreateDateTimeInterval();
            
            return new CreateGoalRequest
            (
                Title: faker.Lorem.Sentence(5),
                Description: faker.Lorem.Sentence(15),
                AimDateStart: range.Start,
                AimDateEnd: range.End
            );
        });

    public static DateTimeInterval CreateDateTimeInterval()
    {
        Faker faker = new();

        ushort days = faker.Random.UShort();
        ushort daysAfter = faker.Random.UShort(min: days);

        DateTime start = faker.Date.Soon(days);
        DateTime end = faker.Date.Soon(daysAfter, start);

        return new DateTimeInterval
        (
            start: start,
            end: end
        ); 
    }


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