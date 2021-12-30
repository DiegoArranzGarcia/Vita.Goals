using System;
using Vita.Goals.Domain.ValueObjects;
using Xunit;

namespace Vita.Goals.Domain.Tests.ValueObjects.DateTimeIntervals
{
    public class DateTimeIntervalTests
    {
        [Theory]
        [MemberData(nameof(DateTimeIntervalTestsFixture.GetValidIntervalDates), MemberType = typeof(DateTimeIntervalTestsFixture))]
        public void GivenValidDates_WhenCreatingDateTimeInterval_ShouldCreateIt(DateTimeOffset start, DateTimeOffset end)
        {
            var interval = new DateTimeInterval(start, end);

            Assert.Equal(expected: start, actual: interval.Start);
            Assert.Equal(expected: end, actual: interval.End);
        }

        [Fact]
        public void GivenStartDateGreatherThanEndDate_WhenCreatingDateTimeInterval_ShouldThrowException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeInterval(start: DateTimeIntervalTestsFixture.FirstFebrary,
                                                                                    end: DateTimeIntervalTestsFixture.FirstJanuary));
        }
    }
}
