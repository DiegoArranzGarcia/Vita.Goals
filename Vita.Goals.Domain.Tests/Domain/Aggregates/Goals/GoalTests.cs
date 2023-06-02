using Castle.DynamicProxy.Internal;
using System.Reflection;
using Vita.Common;
using Vita.Core.Domain;
using Vita.Goals.Domain.Aggregates.Goals;
using Vita.Goals.UnitTests.Attributes;

namespace Vita.Goals.UnitTests.Domain.Aggregates.Goals;

public class GoalTests
{
    [Theory]
    [AutoMoqData]
    public void GivenGoalAndOtherTitle_WhenChangeTitle_ShouldChangeTitle(Goal goal, string otherTitle)
    {
        goal.Title = otherTitle;

        goal.Title.Should().Be(otherTitle);
    }

    [Theory]
    [AutoMoqMemberData(nameof(GoalTestsFixture.GetValidDescriptions), typeof(GoalTestsFixture))]
    public void GivenOtherDescription_WhenChangeDescription_ShouldChangeDescription(string otherDescription, Goal goal)
    {
        goal.Description = otherDescription;

        goal.Description.Should().Be(expected: otherDescription);
    }

    [Theory]
    [AutoMoqMemberData(nameof(GoalTestsFixture.GetValidAimDate), typeof(GoalTestsFixture))]
    public void GivenOtherAimDate_WhenChangeAimDate_ShouldChangeAimDate(DateTimeInterval? otherAimDate, Goal goal)
    {
        goal.AimDate = otherAimDate;

        goal.AimDate.Should().Be(otherAimDate);
    }

    [Fact]
    public void GivenEmptyTitle_WhenCreateGoal_ShouldThrowArgumentException()
    {
        Action act = () => _ = new Goal(string.Empty, GoalTestsFixture.ValidCreatedBy);

        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [AutoMoqData]
    public void GivenEmptyTitle_WhenChangeTitle_ShouldThrowArgumentException(Goal goal)
    {
        Action act = () => goal.Title = string.Empty;

        act.Should().ThrowExactly<ArgumentException>();
    }

    [Fact]
    public void GivenEmptyGuidCreatedBy_WhenCreateGoal_ShouldThrowArgumentException()
    {
        Action act = () => _ = new Goal(GoalTestsFixture.ValidTitle, Guid.Empty);

        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [AutoMoqData]
    public void GivenGoal_WhenCompleting_ShouldChangeTheStatusToCompleted(Goal goal)
    {
        goal.Complete();

        goal.Status.Should().Be(GoalStatus.Completed);
    }
}