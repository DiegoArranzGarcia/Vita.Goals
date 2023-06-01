using Bogus;
using Vita.Common;
using Vita.Goals.Domain.ValueObjects;

namespace Vita.Goals.FunctionalTests.Fixtures.Builders;
public class DateTimeIntervalBuilder
{
    private readonly Faker _faker = new();

    public DateTimeInterval Build()
    {
        ushort days = _faker.Random.UShort();
        ushort daysAfter = _faker.Random.UShort(min: days);

        DateTime start = _faker.Date.Soon(days);
        DateTime end = _faker.Date.Soon(daysAfter, start);

        return new DateTimeInterval
        (
            start: start,
            end: end
        );
    }
}
