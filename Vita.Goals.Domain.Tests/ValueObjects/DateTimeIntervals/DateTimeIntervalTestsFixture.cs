using System;
using System.Collections.Generic;

namespace Vita.Goals.Domain.Tests.ValueObjects.DateTimeIntervals;

public static class DateTimeIntervalTestsFixture
{
    public static DateTimeOffset FirstJanuary => new(year: 2021,
                                                    month: 1,
                                                      day: 1,
                                                     hour: 0,
                                                   minute: 0,
                                                   second: 0,
                                                           TimeSpan.Zero);
    public static DateTimeOffset FirstFebrary => new(year: 2021,
                                                    month: 2,
                                                      day: 1,
                                                     hour: 0,
                                                   minute: 0,
                                                   second: 0,
                                                           TimeSpan.Zero);
    public static IEnumerable<object[]> GetValidIntervalDates()
    {
        yield return new object[] { FirstJanuary, FirstFebrary };
        yield return new object[] { FirstJanuary, FirstJanuary };
    }
}
