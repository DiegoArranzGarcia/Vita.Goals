using Vita.Common;
using Vita.Goals.Domain.ValueObjects;

namespace Vita.Goals.UnitTests.Domain.ValueObjects.DateTimeIntervals;

public class DateTimeIntervalTests
{
    [Theory]
    [MemberData(nameof(DateTimeIntervalTestsFixture.GetValidIntervalDates), MemberType = typeof(DateTimeIntervalTestsFixture))]
    public void GivenValidDates_WhenCreatingDateTimeInterval_ShouldCreateIt(DateTimeOffset start, DateTimeOffset end)
    {
        var interval = new DateTimeInterval(start, end);

        interval.Start.Should().Be(start);
        interval.End.Should().Be(end);
    }

    [Fact]
    public void GivenStartDateGreatherThanEndDate_WhenCreatingDateTimeInterval_ShouldThrowException()
    {
        //Act
        Action act = () => _ = new DateTimeInterval(start: DateTimeIntervalTestsFixture.FirstFebrary,
                                                      end: DateTimeIntervalTestsFixture.FirstJanuary);

        act.Should().ThrowExactly<ArgumentOutOfRangeException>();
    }
}
