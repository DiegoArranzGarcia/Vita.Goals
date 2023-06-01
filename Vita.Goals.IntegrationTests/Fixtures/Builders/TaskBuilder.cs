using Bogus;
using Vita.Common;
using Vita.Goals.Domain.Aggregates.Goals;
using Vita.Goals.Domain.ValueObjects;

namespace Vita.Goals.FunctionalTests.Fixtures.Builders;
public class TaskBuilder
{
    private readonly Faker<Domain.Aggregates.Tasks.Task> _faker = new();
    private Goal Goal { get; set; } = new GoalBuilder().Build();
    private DateTimeInterval? PlannedDate { get; set; }

    public TaskBuilder WithGoal(Goal goal)
    {
        Goal = goal;
        return this;
    }

    public TaskBuilder WithPlannedDate(DateTimeInterval range)
    {
        PlannedDate = range;
        return this;
    }

    public Domain.Aggregates.Tasks.Task Build()
    {
        return _faker.CustomInstantiator(faker => new Domain.Aggregates.Tasks.Task
            (
                title: faker.Lorem.Sentence(5),
                associatedTo: Goal!,
                plannedDate: PlannedDate
            )).Generate();
    }

}
