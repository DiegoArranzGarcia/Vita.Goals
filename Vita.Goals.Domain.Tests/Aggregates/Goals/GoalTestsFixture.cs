using AutoFixture;
using System;
using System.Collections.Generic;
using Vita.Goals.Domain.Aggregates.Goals;
using Vita.Goals.Domain.Tests.AutoFixture;
using Vita.Goals.Domain.ValueObjects;

namespace Vita.Goals.Domain.Tests.Aggregates.Goals
{
    public static class GoalTestsFixture
    {
        private static readonly IFixture _fixture = new Fixture().Customize(new DateTimeIntervalCustomization());

        internal static string GoodTitle { get; private set; }
        internal static string GoodDescription { get; private set; }
        internal static Guid GoodCreatedBy { get; private set; }
        internal static DateTimeInterval GoodAimDate { get; private set; }

        static GoalTestsFixture()
        {
            GoodTitle = "Title";
            GoodDescription = "Description";
            GoodCreatedBy = Guid.NewGuid();
            GoodAimDate = _fixture.Create<DateTimeInterval>();
        }

        internal static Goal CreateGoal()
        {
            return _fixture.Build<Goal>()
                           .Without(x => x.Events)
                           .Without(x => x.Tasks)
                           .Create();
        }

        public static IEnumerable<object[]> GetValidArgumentsCombination()
        {
            yield return new object[] { GoodDescription, GoodAimDate };
            yield return new object[] { GoodDescription, null };
            yield return new object[] { null, GoodAimDate };
            yield return new object[] { null, null };
        }

        public static IEnumerable<object[]> GetValidDescriptions()
        {
            yield return new object[] { null };
            yield return new object[] { string.Empty };
            yield return new object[] { _fixture.Create<string>() };
        }

        public static IEnumerable<object[]> GetValidAimDate()
        {
            yield return new object[] { null };
            yield return new object[] { _fixture.Create<DateTimeInterval>() };
        }

    }
}
