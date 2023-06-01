using AutoFixture.Kernel;
using Vita.Common;

namespace Vita.Goals.UnitTests.AutoFixture.SpecimenBuilders;

public class DateTimeIntervalSpecimenBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        var requestAsType = request as Type;
        if (typeof(DateTimeInterval).Equals(requestAsType))
        {
            var times = context.CreateMany<DateTime>(count: 2);
            return new DateTimeInterval(times.Min(), times.Max());
        }

        return new NoSpecimen();
    }
}
