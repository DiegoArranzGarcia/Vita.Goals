using Bogus;
using Vita.Goals.Domain.Aggregates.Goals;
using Vita.Goals.Domain.ValueObjects;

namespace Vita.Goals.FunctionalTests.Fixtures.Builders;
public class GoalBuilder
{
    private readonly Faker<Goal> _faker = new();
    private Guid UserId { get; set; } = UserBuilder.AliceUserId;
    private DateTimeInterval? Range { get; set; }

    public GoalBuilder WithUserId(Guid userId)
    {
        UserId = userId;
        return this;
    }

    public GoalBuilder WithRange(DateTimeInterval range)
    {
        Range = range;
        return this;
    }

    public Goal Build()
    {
        return _faker.CustomInstantiator(faker => new Goal
            (
                title: faker.Lorem.Sentence(5),
                createdBy: UserId,
                description: faker.Lorem.Sentence(15),
                aimDate: Range
            ))
            .Generate();
    }
}
