using AutoFixture;
using System;
using Vita.Goals.Domain.Aggregates.Goals;
using Vita.Goals.Domain.ValueObjects;
using Xunit;

namespace Vita.Goals.Domain.Tests.Aggregates.Goals
{
    public class GoalTests
    {
        [Theory]
        [MemberData(nameof(GoalTestsFixture.GetValidArgumentsCombination), MemberType = typeof(GoalTestsFixture))]
        public void GivenValidArgs_WhenCreateGoal_ShouldCreateGoal(string description, DateTimeInterval aimDate)
        {
            var goal = new Goal(GoalTestsFixture.GoodTitle, GoalTestsFixture.GoodCreatedBy, description, aimDate);

            Assert.NotNull(goal);
            Assert.NotEqual(expected: Guid.Empty, actual: goal.Id);
            Assert.Equal(expected: GoalTestsFixture.GoodTitle, actual: goal.Title);
            Assert.Equal(expected: description, actual: goal.Description);
            Assert.Equal(expected: GoalTestsFixture.GoodCreatedBy, actual: goal.CreatedBy);
            Assert.Equal(expected: GoalStatus.ToBeDefined, actual: goal.GoalStatus);
            Assert.Equal(expected: aimDate, goal.AimDate);
        }

        [Fact]
        public void GivenOtherTitle_WhenChangeTitle_ShouldChangeTitle()
        {
            var fixture = new Fixture();
            var otherTitle = fixture.Create<string>();

            var goal = GoalTestsFixture.CreateGoal();

            goal.Title = otherTitle;

            Assert.Equal(expected: otherTitle, actual: goal.Title);
        }

        [Theory]
        [MemberData(nameof(GoalTestsFixture.GetValidDescriptions), MemberType = typeof(GoalTestsFixture))]
        public void GivenOtherDescription_WhenChangeDescription_ShouldChangeDescription(string otherDescription)
        {
            var goal = GoalTestsFixture.CreateGoal();

            goal.Description = otherDescription;

            Assert.Equal(expected: otherDescription, actual: goal.Description);
        }

        [Theory]
        [MemberData(nameof(GoalTestsFixture.GetValidAimDate), MemberType = typeof(GoalTestsFixture))]
        public void GivenOtherAimDate_WhenChangeAimDate_ShouldChangeAimDate(DateTimeInterval otherAimDate)
        {
            var goal = GoalTestsFixture.CreateGoal();

            goal.AimDate = otherAimDate;

            Assert.Equal(expected: otherAimDate, actual: goal.AimDate);
        }

        [Fact]
        public void GivenToDoGoal_WhenCompleteGoal_ShouldChangeToCompleted()
        {
            var goal = GoalTestsFixture.CreateGoal();

            goal.Complete();

            Assert.Equal(expected: GoalStatus.Completed, actual: goal.GoalStatus);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void GivenBadTitle_WhenCreateGoal_ShouldThrowArgumentException(string badTitle)
        {
            Assert.ThrowsAny<ArgumentException>(() => new Goal(badTitle, GoalTestsFixture.GoodCreatedBy));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void GivenBadTitle_WhenChangeTitle_ShouldThrowArgumentException(string badTitle)
        {
            var goal = GoalTestsFixture.CreateGoal();

            Assert.ThrowsAny<ArgumentException>(() => goal.Title = badTitle);
        }

        [Fact]
        public void GivenEmptyGuidCreatedBy_WhenCreateGoal_ShouldThrowArgumentException()
        {
            Assert.ThrowsAny<ArgumentException>(() => new Goal(GoalTestsFixture.GoodTitle, Guid.Empty));
        }

        [Fact]
        public void GivenCompletedGoal_WhenCompleteGoal_ShouldThrowArgumentException()
        {
            var goal = GoalTestsFixture.CreateGoal();

            goal.Complete();

            Assert.Throws<ArgumentException>(() => goal.Complete());
        }
    }
}