using AutoFixture.Kernel;
using Vita.Common;
using Vita.Goals.Api.Endpoints.Goals.Create;

namespace Vita.Goals.UnitTests.AutoFixture.SpecimenBuilders;

public class DateTimeIntervalSpecimenBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is not Type type || type != typeof(DateTimeInterval))
            return new NoSpecimen();
        
        var times = context.CreateMany<DateTime>(count: 2);
        return new DateTimeInterval(times.Min(), times.Max());       
    }
}
