using Bogus;
using Vita.Goals.Api.Endpoints.Goals.Create;
using Vita.Goals.Domain.Aggregates.Goals;
using Vita.Goals.FunctionalTests.Fixtures.WebApplicationFactories;
using Vita.Goals.Infrastructure.Sql;

namespace Vita.Goals.FunctionalTests.Goals;

public class GoalsTestsFixture : WebApiApplicationFactory
{
    public async Task<IReadOnlyCollection<Goal>> GoalsInTheDatabase(Guid userId)
    {
        using IServiceScope scope = Services.CreateScope();
        GoalsDbContext context = scope.ServiceProvider.GetRequiredService<GoalsDbContext>();

        List<Goal> goals = new Faker<Goal>().CustomInstantiator((faker) => new Goal(title: faker.Lorem.Sentence(5), createdBy: userId, description: faker.Lorem.Sentence(15)))
                                            .Generate(count: 15);

        context.Goals.AddRange(goals);
        await context.SaveChangesAsync();

        return goals;
    }

    public CreateGoalRequest BuildCreateGoalRequest() => new Faker<CreateGoalRequest>()
        .CustomInstantiator((faker) =>
        {
            DateTime start = faker.Date.Soon(7);
            DateTime end = faker.Date.Soon(10, start);

            return new CreateGoalRequest
            (
                Title: faker.Lorem.Sentence(5),
                Description: faker.Lorem.Sentence(15),
                AimDateStart: new DateTimeOffset(start),
                AimDateEnd: new DateTimeOffset(end)
            );
        });    
}